using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Media;
using Domain.Entities.Posts;
using Microsoft.AspNetCore.Http;
using Service.Specifications;
using Service.Specifications.PostSpecifications;
using ServiceAbstraction.Contracts;
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
        private readonly IAIModerationManager _aiModerationManager;


        public PostService(IUnitOfWork unitOfWork, IMapper mapper, IAIModerationManager aiModerationManager, IFileStorageService fileStorage)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _aiModerationManager = aiModerationManager;
        }

        public async Task<CreatePostResultDTO> CreatePostAsync(CreatePostDTO dto, string userId, List<IFormFile> mediaFiles)
        {
            var post = _mapper.Map<Post>(dto);
            post.CreatedAt = DateTime.UtcNow;
            post.UserId = userId;

            if (string.IsNullOrWhiteSpace(post.Caption) && !mediaFiles.Any())
            {
                return new CreatePostResultDTO
                {
                    IsCreated = false,
                    Status = ContentStatus.Denied,
                    Message = "Post must contain text or media."
                };
            }

            foreach (var file in mediaFiles)
            {
                var savedPath = await _fileStorage.SaveAsync(file);

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
                return new CreatePostResultDTO
                {
                    IsCreated = false,
                    Status = ContentStatus.Denied,
                    Message = "Post violates community guidelines."
                };
            }

            #endregion

            return new CreatePostResultDTO
            {
                IsCreated = true,
                Status = ContentStatus.Accepted,
                Message = "Post created successfully."
            };

        }

        public async Task<PagedResult<PostFeedDTO>> GetFeedAsync(string userId, int page, int pageSize)
        {
            var spec = new PostFeedSpecification(userId, page, pageSize);

            var posts = await _unitOfWork
                .GetRepository<Post, int>()
                .GetAllAsync(spec);

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

            return new PagedResult<PostFeedDTO>
            {
                Items = ordered.Select(x =>
                {
                    var dto = _mapper.Map<PostFeedDTO>(x.Post);
                    dto.IsLikedByCurrentUser = x.IsLikedByUser;
                    dto.FeedScore = x.FeedScore;
                    return dto;
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = ordered.Count == pageSize
            };
        }

        public async Task<PagedResult<PostFeedDTO>> GetGroupFeedAsync(string userId, int groupId, int page, int pageSize)
        {
            var spec = new GroupPostFeedSpecification(userId, groupId, page, pageSize);

            var posts = await _unitOfWork
                .GetRepository<Post, int>()
                .GetAllAsync(spec);

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

            return new PagedResult<PostFeedDTO>
            {
                Items = ordered.Select(x =>
                {
                    var dto = _mapper.Map<PostFeedDTO>(x.Post);
                    dto.IsLikedByCurrentUser = x.IsLikedByUser;
                    dto.FeedScore = x.FeedScore;
                    return dto;
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = ordered.Count == pageSize
            };

        }

        public async Task<bool> LikePostAsync(int postId, string userId)
        {
            var likeRepo = _unitOfWork.GetRepository<Like, int>();

            var spec = new LikeByPostAndUserSpecification(postId, userId);

            var existingLike = await likeRepo.GetByIdAsync(spec);

            if (existingLike is null)
            {
                // like
                var like = new Like
                {
                    PostId = postId,
                    UserId = userId
                };

                await likeRepo.AddAsync(like);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }

            // unlike
            likeRepo.Delete(existingLike);
            await _unitOfWork.SaveChangesAsync();

            return false;
        }

        public async Task<PagedResult<LikeResultDTO>> GetLikesByPostIdAsync(int postId, int page, int pageSize)
        {
            var spec = new LikesByPostIdSpecification(postId, page , pageSize);

            var likes = await _unitOfWork
                .GetRepository<Like, int>()
                .GetAllAsync(spec);

            var totalCount = likes.Count();

            var mapped = _mapper.Map<List<LikeResultDTO>>(likes);

            return new PagedResult<LikeResultDTO>
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

        //    await _unitOfWork
        //        .GetRepository<Comment, int>()
        //        .AddAsync(comment);

        //    return (await _unitOfWork.SaveChangesAsync());

        //}

        //public async Task<PagedResult<CommentResultDTO>> GetCommentsByPostIdAsync(int postId, int page, int pageSize)
        //{
        //    var spec = new CommentsByPostIdSpecification(postId);

        //    var comments = await _unitOfWork
        //        .GetRepository<Comment, int>()
        //        .GetAllAsync(spec);

        //    var totalCount = comments.Count();

        //    var pagedComments = comments
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize + 1)
        //        .ToList();

        //    var hasMore = pagedComments.Count > pageSize;

        //    var finalComments = pagedComments
        //        .Take(pageSize)
        //        .ToList();

        //    var mapped = _mapper.Map<List<CommentResultDTO>>(finalComments);

        //    return new PagedResult<CommentResultDTO>
        //    {
        //        Items = mapped,
        //        Page = page,
        //        PageSize = pageSize,
        //        TotalCount = totalCount,
        //        HasMore = hasMore
        //    };

        //}

    }
}