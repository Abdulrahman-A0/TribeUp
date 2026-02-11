using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Entities.Media;
using Domain.Entities.Posts;
using Domain.Exceptions;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.ModerationExceptions;
using Domain.Exceptions.PostExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.Exceptions.ValidationExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Service.Specifications.GroupMemberSpecs;
using Service.Specifications.ModerationSpecifications;
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

        private readonly IGenericRepository<Tag, int> tagRepo;
        private readonly IGenericRepository<Post, int> postRepo;
        private readonly IGenericRepository<Like, int> likeRepo;
        private readonly IGenericRepository<Group, int> groupRepo;
        private readonly IGenericRepository<Comment, int> commentRepo;
        private readonly IGenericRepository<AIModeration, int> moderationRepo;

        private readonly int PostPoints = 5;
        private readonly int CommentPoints = 1;

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

            tagRepo = _unitOfWork.GetRepository<Tag, int>();
            postRepo = _unitOfWork.GetRepository<Post, int>();
            likeRepo = _unitOfWork.GetRepository<Like, int>();
            groupRepo = _unitOfWork.GetRepository<Group, int>();
            commentRepo = _unitOfWork.GetRepository<Comment, int>();
            moderationRepo = _unitOfWork.GetRepository<AIModeration, int>();
        }

        public async Task<CreateEntityResultDTO> CreatePostAsync(
            string userId, 
            string username,
            CreatePostDTO dto,
            List<IFormFile> mediaFiles)
        {
            var post = _mapper.Map<Post>(dto);
            post.Caption = post.Caption?.Trim();
            post.CreatedAt = DateTime.UtcNow;
            post.UserId = userId;

            var group = await groupRepo.GetByIdAsync(dto.GroupId)
               ?? throw new GroupNotFoundException(dto.GroupId);

            if (string.IsNullOrWhiteSpace(post.Caption) && !mediaFiles.Any())
                throw new PostAndCommentContentValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["PostContent"] = new[] { "Post must contain text or media." }
                    });

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


            foreach (var taggedUserId in dto.TaggedUserIds.Distinct())
            {
                var tag = new Tag
                {
                    PostId = post.Id,
                    UserId = taggedUserId
                };
                await tagRepo.AddAsync(tag);

                await _notificationService.CreateAsync(new CreateNotificationDTO
                {
                    RecipientUserId = taggedUserId,
                    ActorUserId = userId,
                    Type = NotificationType.PostTag,
                    Title = "You were tagged in a post",
                    Message = $"{username} tagged you in his post: \"{GetFirstWord(post.Caption)}\"",
                    ReferenceId = post.Id
                });
            }

            await _groupScoreService.IncreaseOnActionAsync(dto.GroupId, PostPoints);
            await _unitOfWork.SaveChangesAsync();

            return new CreateEntityResultDTO
            {
                IsCreated = true,
                Status = ContentStatus.Accepted,
                Message = "Post created successfully."
            };

        }

        // not working yet
        public async Task<CreateEntityResultDTO> UpdatePostAsync(
           string userId,
           string username,
           int postId,
           CreatePostDTO dto,
           List<IFormFile> mediaFiles)
        {
            var moderation = moderationRepo.AsQueryable();
            var spec = new PostByIdSpecification(userId, moderation, postId);
            var post = await postRepo.GetByIdAsync(spec)
                ?? throw new PostNotFoundException(postId);

            if (string.IsNullOrWhiteSpace(post.Caption) && !mediaFiles.Any())
                throw new ValidationException("Post must contain text or media.");

            foreach (var file in mediaFiles)
            {
                var savedPath = await _fileStorage.SaveAsync(file, MediaType.PostMedia);

                //if (post.MediaItems.ElementAt(true))

                post.MediaItems.Add(new MediaItem
                {
                    MediaURL = savedPath,
                    UploadedAt = DateTime.UtcNow,
                    Type = file.ContentType.StartsWith("video") ? "Video" : "Image",
                    Order = post.MediaItems.Count
                });
            }
            

            //await postRepo.AddAsync(post);

            //await _unitOfWork.SaveChangesAsync();


            foreach (var taggedUserId in dto.TaggedUserIds.Distinct())
            {
                var tag = new Tag
                {
                    PostId = post.Id,
                    UserId = taggedUserId
                };

                await tagRepo.AddAsync(tag);

                await _notificationService.CreateAsync(new CreateNotificationDTO
                {
                    RecipientUserId = taggedUserId,
                    ActorUserId = userId,
                    Type = NotificationType.PostTag,
                    Title = "You were tagged in a post",
                    Message = $"tagged you in his post: \"{GetFirstWord(post.Caption)}\"",
                    ReferenceId = post.Id
                });
            }

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

        public async Task<DeleteEntityResultDTO> DeletePostAsync(
            string userId, 
            int postId)
        {
            var moderation = moderationRepo.AsQueryable();
            var spec = new PostByIdSpecification(userId, moderation, postId);
            var post  = await postRepo.GetByIdAsync(spec)
                ?? throw new PostNotFoundException(postId);

            if (post.User.Id != userId)
                if (!await IsAdminAsync(userId, post.GroupId))
                    throw new ForbiddenActionException();
            
            await _groupScoreService.DecreaseOnActionAsync(post.GroupId, PostPoints);
            postRepo.Delete(post);
            await _unitOfWork.SaveChangesAsync();
            return new DeleteEntityResultDTO
            {
                Message = "Deleted successfully"
            };

        }


        public async Task<PostDTO> GetPostByIdAsync(
            string userId, 
            int postId)
        {
            var moderation = moderationRepo.AsQueryable();

            var spec = new PostByIdSpecification(userId, moderation, postId);
            var post = await postRepo.GetByIdAsync(spec) 
                ?? throw new PostNotFoundException(postId);

            var mapped = _mapper.Map<PostDTO>(post);

            return mapped;
        }


        public async Task<PagedResult<PostDTO>> GetPersonalFeedAsync(
           string userId,
           int page,
           int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new PageIndexAndPageSizeValidationException(page, pageSize);

            var moderation = moderationRepo.AsQueryable();
            var spec = new PersonalPostFeedSpecification(userId, moderation, page, pageSize);
            var posts = await postRepo.GetAllAsync(spec);

            var postIds = posts.Select(p => p.Id).ToList();
            var deniedPostIds = await GetDeniedPostIdsAsync(postIds);
            var totalCount = await GetVisiblePostsCountAsync(userId);
            var ordered = posts
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return new PagedResult<PostDTO>
            {
                Items = ordered.Select(x =>
                {
                    var dto = _mapper.Map<PostDTO>(x);
                    dto.IsLikedByCurrentUser = x.Likes.Any(l => l.UserId == userId);
                    dto.IsDenied =
                    deniedPostIds.Contains(x.Id) &&
                        x.UserId == userId;
                    return dto;
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = totalCount > page * pageSize
            };
        }


        public async Task<PagedResult<PostDTO>> GetFeedAsync(
            string userId,
            int page, 
            int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new PageIndexAndPageSizeValidationException(page, pageSize);

            var moderation = moderationRepo.AsQueryable();
            var spec = new PostFeedSpecification(userId, moderation, page, pageSize);
            var posts = await postRepo.GetAllAsync(spec);

            var postIds = posts.Select(p => p.Id).ToList();
            var deniedPostIds = await GetDeniedPostIdsAsync(postIds);
            var totalCount = await GetVisiblePostsCountAsync(userId);
            var scored = posts.Select(post =>
            {
                var (feedScore, isLiked) = CalculateFeedScore(post, userId);

                return new
                {
                    Post = post,
                    FeedScore = feedScore,
                    IsLikedByUser = isLiked
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
                    dto.IsDenied =
                    deniedPostIds.Contains(x.Post.Id) &&
                        x.Post.UserId == userId;
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
                throw new PageIndexAndPageSizeValidationException(page, pageSize);

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var moderation = moderationRepo.AsQueryable();
            var spec = new GroupPostFeedSpecification(userId, groupId, moderation, page, pageSize);
            var posts = await postRepo.GetAllAsync(spec);
            
            var postIds = posts.Select(p => p.Id).ToList();
            var deniedPostIds = await GetDeniedPostIdsAsync(postIds);
            var totalCount = await GetVisiblePostsCountAsync(userId, groupId);
            var scored = posts.Select(post =>
            {
                var (feedScore, isLiked) = CalculateFeedScore(post, userId);
                return new
                {
                    Post = post,
                    FeedScore = feedScore,
                    IsLikedByUser = isLiked
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
                    dto.IsDenied =
                    deniedPostIds.Contains(x.Post.Id) &&
                        x.Post.UserId == userId;
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
            string username,
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
                    Message = $"{username} liked your post: \"{GetFirstWord(post.Caption)}\"",
                    ReferenceId = post.Id
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
                throw new PageIndexAndPageSizeValidationException(page, pageSize);

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
            string username,
            int postId, 
            CommentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new PostAndCommentContentValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["CommentContent"] = new[] { "Comment must contain text." }
                    });

            var post = await postRepo.GetByIdAsync(postId)
                ?? throw new PostNotFoundException(postId);

            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = dto.Content.Trim()
            };
            await commentRepo.AddAsync(comment);
            
            await _unitOfWork.SaveChangesAsync();

            # region AI moderation result

            var moderationResult = await _aiModerationManager.ModerateAsync(
                comment.Content ?? string.Empty,
                ModeratedEntityType.Comment,
                comment.Id);

            if (!moderationResult.IsAccepted)
            {
                return new CreateEntityResultDTO
                {
                    IsCreated = false,
                    Status = ContentStatus.Denied,
                    Message = "Comment violates community guidelines."
                };
            }

            #endregion

            await _notificationService.CreateAsync(new CreateNotificationDTO
            {
                RecipientUserId = post.UserId,
                ActorUserId = userId,
                Type = NotificationType.PostComment,
                Title = $"New comment on your post: \"{GetFirstWord(post.Caption)}\"",
                Message = $"{username} commented: \"{GetFirstWord(comment.Content)}\"",
                ReferenceId = post.Id
            });

            await _groupScoreService.IncreaseOnActionAsync(post.GroupId, CommentPoints);
            await _unitOfWork.SaveChangesAsync();

            return new CreateEntityResultDTO
            {
                IsCreated = true,
                Status = ContentStatus.Accepted,
                Message = "Comment created successfully"
            };

        }

        public async Task<CreateEntityResultDTO> UpdateCommentAsync(
            string userId,
            string username,
            int commentId, 
            CommentDTO dto)
        {
            var moderatedSpec = new ChangeEntityStatusSpecification(ModeratedEntityType.Comment, commentId);
            var moderatedComment = await moderationRepo.GetByIdAsync(moderatedSpec);
            
            var spec = new CommentByIdSpecification(commentId);
            var comment = await commentRepo.GetByIdAsync(spec)
                ?? throw new CommentNotFoundException(commentId);

            if (userId != comment.UserId)
                throw new ForbiddenActionException();

            comment.Content = dto.Content;

            # region AI moderation result

            var moderationResult = await _aiModerationManager.ModerateAsync(
                comment.Content ?? string.Empty,
                ModeratedEntityType.Comment,
                comment.Id);

            if (!moderationResult.IsAccepted)
            {
                return new CreateEntityResultDTO
                {
                    IsCreated = false,
                    Status = ContentStatus.Denied,
                    Message = "Comment violates community guidelines."
                };
            }

            #endregion

            commentRepo.Update(comment);            
            await _unitOfWork.SaveChangesAsync();

            return new CreateEntityResultDTO
            {
                IsCreated = true,
                Status = ContentStatus.Accepted,
                Message = "Comment updated successfully"
            };
        }


        public async Task<PagedResult<CommentResultDTO>> GetCommentsByPostIdAsync(
            string userId,
            int postId, 
            int page, 
            int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new PageIndexAndPageSizeValidationException(page, pageSize);
            
            var post = await postRepo.GetByIdAsync(postId)
                ?? throw new PostNotFoundException(postId);
            
            var moderation = moderationRepo.AsQueryable();

            var spec = new CommentsByPostIdSpecification(userId, moderation, postId, page, pageSize);
            var comments = await commentRepo.GetAllAsync(spec);
           
            var totalCount = await commentRepo.CountAsync(c => 
                    c.PostId == postId &&
                    !moderation.Any(m =>
                        m.EntityType == ModeratedEntityType.Comment &&
                        m.EntityId == c.Id &&
                        m.Status == ContentStatus.Denied &&
                        c.UserId != userId
                    )
                );

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


        public async Task<DeleteEntityResultDTO> DeleteCommentAsync(
            string userId, 
            int commentId)
        {

            var spec = new CommentByIdSpecification(commentId);
            var comment  = await commentRepo.GetByIdAsync(spec) 
                ?? throw new CommentNotFoundException(commentId);

            var post = await postRepo.GetByIdAsync(comment.PostId)
                ?? throw new PostNotFoundException(comment.PostId);

            if(comment.User.Id != userId)
                if(!await IsAdminAsync(userId, comment.Post.GroupId))
                    throw new ForbiddenActionException();

            await _groupScoreService.DecreaseOnActionAsync(post.GroupId, CommentPoints);
            commentRepo.Delete(comment);
            await _unitOfWork.SaveChangesAsync();
            return new DeleteEntityResultDTO
            {
                Message = "Deleted successfully"
            };

        }
       

        public async Task<PagedResult<PostDTO>> GetDeniedPostsByGroupIdAsync(
            string userId,
            int groupId, 
            int page, 
            int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new PageIndexAndPageSizeValidationException(page, pageSize);

            if (!await IsAdminAsync(userId, groupId))
                throw new ForbiddenActionException();

            var moderation = moderationRepo.AsQueryable();
            var spec = new GroupDeniedPostsSpecification(groupId, moderation, page, pageSize);
            var posts = await postRepo.GetAllAsync(spec)
                ?? throw new GroupNotFoundException(groupId);
            var totalCount = await postRepo.CountAsync(p =>
                    p.GroupId == groupId &&
                    moderation.Any(m =>
                        m.EntityType == ModeratedEntityType.Post &&
                        m.EntityId == p.Id &&
                        m.Status == ContentStatus.Denied
                    )
            );

            var ordered = posts.OrderByDescending(p => p.CreatedAt).ToList();

            return new PagedResult<PostDTO>
            {
                Items = ordered.Select(x =>
                {
                    var dto = _mapper.Map<PostDTO>(x);
                    dto.IsDenied = true;
                    return dto;
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = totalCount > page * pageSize
            };
        }


        public async Task<CreateEntityResultDTO> ChangeModerationStatusAsync(
            string userId, 
            int groupId,
            ModerationDTO dto)
        {
            if (!await IsAdminAsync(userId, groupId))
                throw new ForbiddenActionException();

            var group = await groupRepo.GetByIdAsync(groupId)
               ?? throw new GroupNotFoundException(groupId);

            var spec = new ChangeEntityStatusSpecification(dto.EntityType, dto.EntityId);
            var entity = await moderationRepo.GetByIdAsync(spec)
                ?? throw new EntityNotFoundException(dto.EntityType, dto.EntityId);

            entity.Status = dto.ContentStatus;
            entity.ReviewedAt = DateTime.UtcNow;

            moderationRepo.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            int points = 0;
            switch (dto.EntityType)
            {
                case ModeratedEntityType.Post:
                    points = 5;
                    break;
                case ModeratedEntityType.Comment:
                    points = 1;
                    break;
            }

            if (dto.ContentStatus == ContentStatus.Accepted)
            {
                await _groupScoreService.IncreaseOnActionAsync(groupId, points);
                await _unitOfWork.SaveChangesAsync();

                return new CreateEntityResultDTO
                {
                    IsCreated = true,
                    Status = dto.ContentStatus,
                    Message = "Accepted by Admins."
                };
            }

            await _groupScoreService.DecreaseOnActionAsync(groupId, points);
            await _unitOfWork.SaveChangesAsync();

            return new CreateEntityResultDTO
            {
                IsCreated = false,
                Status = dto.ContentStatus,
                Message = "Denied by Admins."
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

        private (double FeedScore, bool IsLikedByUser) CalculateFeedScore(Post post, string userId)
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

            return (feedScore, likedByUser);
        }

        private async Task<List<int>> GetDeniedPostIdsAsync(List<int> postIds)
        {
            return await moderationRepo.AsQueryable()
                .Where(m =>
                    m.EntityType == ModeratedEntityType.Post &&
                    m.Status == ContentStatus.Denied &&
                    postIds.Contains(m.EntityId)
                )
                .Select(m => m.EntityId)
                .ToListAsync();
        }

        private async Task<int> GetVisiblePostsCountAsync(string userId, int? groupId = null)
        {
            var moderation = moderationRepo.AsQueryable();

            return await postRepo.CountAsync(p =>
                (groupId == null || p.GroupId == groupId) &&
                !moderation.Any(m =>
                    m.EntityType == ModeratedEntityType.Post &&
                    m.EntityId == p.Id &&
                    m.Status == ContentStatus.Denied &&
                    p.UserId != userId
                )
            );
        }

        #endregion

    }
}