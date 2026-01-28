using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Entities.Media;
using Domain.Entities.Posts;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.PostExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Service.Specifications;
using Service.Specifications.GroupMemberSpecs;
using Service.Specifications.PostSpecifications;
using ServiceAbstraction.Contracts;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;
using Shared.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Service.Implementations
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IGroupScoreService _groupScoreService;
        private readonly IAIModerationManager _aiModerationManager;


        public PostService
            (
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IGroupScoreService groupScoreService,
            IAIModerationManager aiModerationManager
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _groupScoreService = groupScoreService;
            _aiModerationManager = aiModerationManager;
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

            await _unitOfWork
                .GetRepository<Post, int>()
                .AddAsync(post);

            await _unitOfWork.SaveChangesAsync(); // PostId generated here

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

            var post = await _unitOfWork
                .GetRepository<Post, int>()
                .GetByIdAsync(spec) 
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

            var moderation = _unitOfWork
                .GetRepository<AIModeration, int>()
                .AsQueryable();

            var spec = new PostFeedSpecification(userId, moderation, page, pageSize);

            var repo = _unitOfWork.GetRepository<Post, int>();
            
            var posts = await repo.GetAllAsync(spec);

            var totalCount = await repo.CountAsync(p =>
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
            
            var group = await _unitOfWork
                .GetRepository<Domain.Entities.Groups.Group, int>()
                .GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var moderation = _unitOfWork
                .GetRepository<AIModeration, int>()
                .AsQueryable();

            var spec = new GroupPostFeedSpecification(userId, groupId, moderation, page, pageSize);
            var repo = _unitOfWork.GetRepository<Post, int>();
            var posts = await repo.GetAllAsync(spec);

            var totalCount = await repo.CountAsync(p =>
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
            var post = await _unitOfWork
                .GetRepository<Post, int>()
                .GetByIdAsync(postId)
                ?? throw new PostNotFoundException(postId);

            var repo = _unitOfWork.GetRepository<Like, int>();
            var spec = new LikeByPostAndUserSpecification(userId, postId);
            var existingLike = await repo.GetByIdAsync(spec);

            if (existingLike is null)
            {
                var like = new Like
                {
                    PostId = postId,
                    UserId = userId
                };

                await repo.AddAsync(like);
                await _unitOfWork.SaveChangesAsync();

                return new ToggleLikeDTO 
                { 
                    Message = "Liked"
                };
            }

            repo.Delete(existingLike);
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
            
            var post = await _unitOfWork
                 .GetRepository<Post, int>()
                 .GetByIdAsync(postId)
                 ?? throw new PostNotFoundException(postId);

            var spec = new LikesByPostIdSpecification(postId, page, pageSize);
            var likes = await _unitOfWork
                .GetRepository<Like, int>()
                .GetAllAsync(spec);

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

        //public async Task<int> AddCommentAsync(int postId, CreateCommentDTO dto, string userId)
        //{
        //    var comment = new Comment
        //    {
        //        PostId = postId,
        //        UserId = userId,
        //        Content = dto.Content
        //    };

        public async Task<CreateEntityResultDTO> AddCommentAsync(
            string userId, 
            int postId, 
            CommentDTO dto)
        {           
            var post = await _unitOfWork
                .GetRepository<Post, int>()
                .GetByIdAsync(postId)
                ?? throw new PostNotFoundException(postId);

            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = dto.Content
            };

            await _unitOfWork
                .GetRepository<Comment, int>()
                .AddAsync(comment);
            
            await _unitOfWork.SaveChangesAsync();

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
            var post = await _unitOfWork
                .GetRepository<Post, int>()
                .GetByIdAsync(postId)
                ?? throw new PostNotFoundException(postId);

            var spec = new CommentsByPostIdSpecification(postId, page, pageSize);
            var repo = _unitOfWork.GetRepository<Comment, int>();
            var comments = await repo.GetAllAsync(spec);
            var totalCount = await repo
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
            var repo = _unitOfWork.GetRepository<Post, int>();
            var post  = await repo.GetByIdAsync(spec)
                ?? throw new PostNotFoundException(postId);

            if(post.User.Id != userId)
                if(!await IsAdminAsync(userId,post.GroupId))
                    return new DeleteEntityResultDTO
                    {
                        Message = "You don't have permission to delete this post"
                    };

            repo.Delete(post);
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
            var repo = _unitOfWork.GetRepository<Comment, int>();
            var comment  = await repo.GetByIdAsync(spec) 
                ?? throw new CommentNotFoundException(commentId);

            if(comment.User.Id != userId)
                if(!await IsAdminAsync(userId, comment.Post.GroupId))
                    return new DeleteEntityResultDTO
                    {
                        Message = "You don't have permission to delete this comment"
                    };

            repo.Delete(comment);
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
            var repo = _unitOfWork.GetRepository<Comment, int>();
            var comment = await repo.GetByIdAsync(spec)
                ?? throw new CommentNotFoundException(commentId);

            comment.Content = dto.Content;

            repo.Update(comment);
            
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


        #endregion

    }
}