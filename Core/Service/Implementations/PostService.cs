using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Entities.Media;
using Domain.Entities.Posts;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.PostExceptions;
using Microsoft.AspNetCore.Http;
using Service.Specifications.GroupMemberSpecs;
using Service.Specifications.PostSpecifications;
using ServiceAbstraction.Contracts;
using Shared.DTOs.NotificationModule;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Service.Implementations
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IGroupScoreService _groupScoreService;
        private readonly INotificationService _notificationService;
        private readonly IAIModerationManager _aiModerationManager;

        private readonly IGenericRepository<Post, int> postRepo;
        private readonly IGenericRepository<Like, int> likeRepo;
        private readonly IGenericRepository<Group, int> groupRepo;
        private readonly IGenericRepository<Comment, int> commentRepo;
        private readonly IGenericRepository<AIModeration, int> moderationRepo;


        public PostService
            (
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IGroupScoreService groupScoreService,
            INotificationService notificationService,
            IAIModerationManager aiModerationManager
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _groupScoreService = groupScoreService;
            _notificationService = notificationService;
            _aiModerationManager = aiModerationManager;

            postRepo = _unitOfWork.GetRepository<Post, int>();
            likeRepo = _unitOfWork.GetRepository<Like, int>();
            groupRepo = _unitOfWork.GetRepository<Group, int>();
            commentRepo = _unitOfWork.GetRepository<Comment, int>();
            moderationRepo = _unitOfWork.GetRepository<AIModeration, int>();
        }

        public async Task<CreateEntityResultDTO> CreatePostAsync(
            string userId,
            CreatePostDTO dto,
            List<IFormFile> mediaFiles)
        {
            var post = _mapper.Map<Post>(dto);
            post.CreatedAt = DateTime.UtcNow;
            post.UserId = userId;

            if (string.IsNullOrWhiteSpace(post.Caption) && !mediaFiles.Any())
                throw new ValidationException("Post must contain text or media.");

            foreach (var file in mediaFiles)
            {
                var savedPath = await _fileStorage.SaveAsync(file, MediaType.PostMedia);

                post.MediaItems.Add(new MediaItem
                {
                    MediaURL = savedPath,
                    UploadedAt = DateTime.UtcNow,
                    Type = file.ContentType.StartsWith("video") ? "Video" : "Image",
                    Order = post.MediaItems.Count
                });
            }

            await postRepo.AddAsync(post);

            await _unitOfWork.SaveChangesAsync();

            # region AI moderation result

            var moderationResult = await _aiModerationManager.ModerateAsync(
                post.Caption ?? string.Empty,
                ModeratedEntityType.Post,
                post.Id);

            if (!moderationResult.IsAccepted)
            {
                return new CreateEntityResultDTO
                {
                    IsCreated = false,
                    Status = ContentStatus.Denied,
                    Message = "Post violates community guidelines."
                };
            }

            #endregion

            //await _groupScoreService.IncreaseOnActionAsync(dto.GroupId, 5);

            return new CreateEntityResultDTO
            {
                IsCreated = true,
                Status = ContentStatus.Accepted,
                Message = "Post created successfully."
            };

        }


        public async Task<PostDTO> GetPostByIdAsync(
            string userId, 
            int postId)
        {
            var spec = new PostByIdSpecification(postId);
            var post = await postRepo.GetByIdAsync(spec) 
                ?? throw new PostNotFoundException(postId);

            var mapped = _mapper.Map<PostDTO>(post);

            return mapped;
        }


        public async Task<PagedResult<PostDTO>> GetFeedAsync(
            string userId,
            int page, 
            int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ValidationException("Page and PageSize must be greater than zero.");

            var moderation = moderationRepo.AsQueryable();
            var spec = new PostFeedSpecification(userId, moderation, page, pageSize);
            var posts = await postRepo.GetAllAsync(spec);

            var totalCount = await postRepo.CountAsync(p =>
                    !moderation.Any(m =>
                        m.EntityType == ModeratedEntityType.Post &&
                        m.EntityId == p.Id &&
                        m.Status == ContentStatus.Denied &&
                        p.UserId != userId
                    )
                );
            
            var scored = posts.Select(post =>
            {
                var member = post.Group.GroupMembers
                    .FirstOrDefault(m => m.UserId == userId);

                int groupPriority = member?.Role switch
                {
                    RoleType.Admin => 60,
                    RoleType.Member => 40,
                    RoleType.Follower => 20,
                    _ => 5
                };

                int engagementScore =
                    (post.Likes.Count * 2) +
                    (post.Comments.Count * 3);

                double ageHours =
                    (DateTime.UtcNow - post.CreatedAt).TotalHours;

                double timePenalty = ageHours * 0.2;

                bool likedByUser =
                    post.Likes.Any(l => l.UserId == userId);

                int likedPenalty = likedByUser ? 15 : 0;

                double feedScore =
                    groupPriority +
                    engagementScore -
                    timePenalty -
                    likedPenalty;

                return new
                {
                    Post = post,
                    FeedScore = feedScore,
                    IsLikedByUser = likedByUser
                };
            });

            var ordered = scored
                .OrderByDescending(x => x.FeedScore)
                .ThenByDescending(x => x.Post.CreatedAt)
                .ToList();

            return new PagedResult<PostDTO>
            {
                Items = ordered.Select(x =>
                {
                    var dto = _mapper.Map<PostDTO>(x.Post);
                    dto.IsLikedByCurrentUser = x.IsLikedByUser;
                    dto.FeedScore = x.FeedScore;
                    return dto;
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = totalCount > page * pageSize
            };
        }


        public async Task<PagedResult<PostDTO>> GetGroupFeedAsync(
            string userId,
            int groupId, 
            int page, 
            int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ValidationException("Page and PageSize must be greater than zero.");
            
            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var moderation = moderationRepo.AsQueryable();
            var spec = new GroupPostFeedSpecification(userId, groupId, moderation, page, pageSize);
            var posts = await postRepo.GetAllAsync(spec);

            var totalCount = await postRepo.CountAsync(p =>
                    !moderation.Any(m =>
                        m.EntityType == ModeratedEntityType.Post &&
                        m.EntityId == p.Id &&
                        m.Status == ContentStatus.Denied &&
                        p.UserId != userId
                    )
                );

            var scored = posts.Select(post =>
            {
                var member = post.Group.GroupMembers
                    .FirstOrDefault(m => m.UserId == userId);

                int groupPriority = member?.Role switch
                {
                    RoleType.Admin => 60,
                    RoleType.Member => 40,
                    RoleType.Follower => 20,
                    _ => 5
                };

                int engagementScore =
                    (post.Likes.Count * 2) +
                    (post.Comments.Count * 3);

                double ageHours =
                    (DateTime.UtcNow - post.CreatedAt).TotalHours;

                double timePenalty = ageHours * 0.2;

                bool likedByUser =
                    post.Likes.Any(l => l.UserId == userId);

                int likedPenalty = likedByUser ? 15 : 0;

                double feedScore =
                    groupPriority +
                    engagementScore -
                    timePenalty -
                    likedPenalty;

                return new
                {
                    Post = post,
                    FeedScore = feedScore,
                    IsLikedByUser = likedByUser
                };
            });

            var ordered = scored
                .OrderByDescending(x => x.FeedScore)
                .ThenByDescending(x => x.Post.CreatedAt)
                .ToList();

            return new PagedResult<PostDTO>
            {
                Items = ordered.Select(x =>
                {
                    var dto = _mapper.Map<PostDTO>(x.Post);
                    dto.IsLikedByCurrentUser = x.IsLikedByUser;
                    dto.FeedScore = x.FeedScore;
                    return dto;
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = totalCount > page * pageSize
            };

        }


        public async Task<ToggleLikeDTO> ToggeleLikePostAsync(
            string userId, 
            int postId)
        {
            var post = await postRepo.GetByIdAsync(postId)
                ?? throw new PostNotFoundException(postId);

            var spec = new LikeByPostAndUserSpecification(userId, postId);
            var existingLike = await likeRepo.GetByIdAsync(spec);

            if (existingLike is null)
            {
                var like = new Like
                {
                    PostId = postId,
                    UserId = userId
                };

                await likeRepo.AddAsync(like);
                await _unitOfWork.SaveChangesAsync();

                await _notificationService.CreateAsync(new CreateNotificationDTO
                {
                    RecipientUserId = post.UserId,
                    ActorUserId = userId,
                    Type = NotificationType.PostLike,
                    Title = $"New like on your post",
                    Message = $"liked your post: \"{GetFirstWord(post.Caption)}\"",
                    ReferenceId = like.Id
                });

                return new ToggleLikeDTO 
                { 
                    Message = "Liked"
                };
            }

            likeRepo.Delete(existingLike);
            await _unitOfWork.SaveChangesAsync();

            return new ToggleLikeDTO 
            { 
                Message = "Unliked"
            };
        }


        public async Task<PagedResult<LikesResultDTO>> GetLikesByPostIdAsync(
            int postId, 
            int page,
            int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ValidationException("Page and PageSize must be greater than zero.");
            
            var post = await postRepo.GetByIdAsync(postId)
                 ?? throw new PostNotFoundException(postId);

            var spec = new LikesByPostIdSpecification(postId, page, pageSize);
            var likes = await likeRepo.GetAllAsync(spec);

            var totalCount = likes.Count();

            var mapped = _mapper.Map<List<LikesResultDTO>>(likes);

            return new PagedResult<LikesResultDTO>
            {
                Items = mapped,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = likes.Count() == pageSize
            };

        }


        public async Task<CreateEntityResultDTO> AddCommentAsync(
            string userId, 
            int postId, 
            CommentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ValidationException("Comment must contain text");

            var post = await postRepo.GetByIdAsync(postId)
                ?? throw new PostNotFoundException(postId);

            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = dto.Content
            };

            await commentRepo.AddAsync(comment);
            
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateAsync(new CreateNotificationDTO
            {
                RecipientUserId = post.UserId,
                ActorUserId = userId,
                Type = NotificationType.PostComment,
                Title = $"New comment on your post: \"{GetFirstWord(post.Caption)}\"",
                Message = $"commented: \"{GetFirstWord(comment.Content)}\"",
                ReferenceId = comment.Id
            });

            return new CreateEntityResultDTO
            {
                IsCreated = true,
                Status = ContentStatus.Accepted,
                Message = "Comment created successfully"
            };

        }


        public async Task<PagedResult<CommentResultDTO>> GetCommentsByPostIdAsync(
            int postId, 
            int page, 
            int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ValidationException("Page and PageSize must be greater than zero.");
            var post = await postRepo.GetByIdAsync(postId)
                ?? throw new PostNotFoundException(postId);

            var spec = new CommentsByPostIdSpecification(postId, page, pageSize);
            var comments = await commentRepo.GetAllAsync(spec);
            var totalCount = await commentRepo
                .CountAsync(c => c.PostId == postId);

            var mapped = _mapper.Map<List<CommentResultDTO>>(comments);

            return new PagedResult<CommentResultDTO>
            {
                Items = mapped,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = totalCount == pageSize
            };

        }


        public async Task<DeleteEntityResultDTO> DeletePostAsync(
            string userId, 
            int postId)
        {
            var spec = new PostByIdSpecification(postId);
            var post  = await postRepo.GetByIdAsync(spec)
                ?? throw new PostNotFoundException(postId);

            if(post.User.Id != userId)
                if(!await IsAdminAsync(userId,post.GroupId))
                    return new DeleteEntityResultDTO
                    {
                        Message = "You don't have permission to delete this post"
                    };

            postRepo.Delete(post);
            await _unitOfWork.SaveChangesAsync();
            return new DeleteEntityResultDTO
            {
                Message = "Deleted successfully"
            };

        }


        public async Task<DeleteEntityResultDTO> DeleteCommentAsync(
            string userId, 
            int commentId)
        {
            var spec = new CommentByIdSpecification(commentId);
            var comment  = await commentRepo.GetByIdAsync(spec) 
                ?? throw new CommentNotFoundException(commentId);

            if(comment.User.Id != userId)
                if(!await IsAdminAsync(userId, comment.Post.GroupId))
                    return new DeleteEntityResultDTO
                    {
                        Message = "You don't have permission to delete this comment"
                    };

            commentRepo.Delete(comment);
            await _unitOfWork.SaveChangesAsync();
            return new DeleteEntityResultDTO
            {
                Message = "Deleted successfully"
            };

        }
       
        
        public async Task<CreateEntityResultDTO> UpdateCommentAsync(
            string userId,
            int commentId, 
            CommentDTO dto)
        {
            var spec = new CommentByIdSpecification(commentId);
            var comment = await commentRepo.GetByIdAsync(spec)
                ?? throw new CommentNotFoundException(commentId);

            comment.Content = dto.Content;

            commentRepo.Update(comment);
            
            await _unitOfWork.SaveChangesAsync();

            return new CreateEntityResultDTO
            {
                IsCreated = true,
                Status = ContentStatus.Accepted,
                Message = "Comment updated successfully"
            };
        }




        #region Private Methods
        private async Task<bool> IsAdminAsync(string userId, int groupId)
        {
            var groupSpec = new GroupAdminsSpec(groupId);
            var groupMemberRepo = _unitOfWork.GetRepository<GroupMember, int>();
            var groupAdmins = await groupMemberRepo.GetAllAsync(groupSpec) ?? throw new GroupNotFoundException(groupId);

            return groupAdmins.Any(a => a.UserId == userId);
        }

        private static string GetFirstWord(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "...";

            return text
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .First() + "...";
        }

        #endregion

    }
}