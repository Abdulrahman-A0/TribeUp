using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Posts;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IContentModerationService _contentModerationService;


        public PostService(IUnitOfWork unitOfWork, IMapper mapper, IContentModerationService contentModerationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contentModerationService = contentModerationService;
        }

        public async Task<CreatePostResultDTO> CreatePostAsync(CreatePostDTO dto, string userId)
        {
            var post = _mapper.Map<Post>(dto);
            post.CreatedAt = DateTime.UtcNow; 
            post.CreatedByUserId = userId;

            if (string.IsNullOrWhiteSpace(post.Caption) && !post.MediaItems.Any())
            {
                return new CreatePostResultDTO
                {
                    IsCreated = false,
                    Status = ContentStatus.Denied,
                    Message = "Post must contian text or media."
                };
            }

            await _unitOfWork
                .GetRepository<Post, int>()
                .AddAsync(post);

            await _unitOfWork.SaveChangesAsync(); // PostId generated here

            // AI moderation
            var moderationResult =
                await _contentModerationService.AnalyzeAsync(dto.Caption ?? string.Empty);

            // If denied ,then save ONLY in AI_Moderation table
            if (!moderationResult.IsAccepted)
            {
                var aiModeration = new AIModeration
                {
                    PostId = post.Id,
                    DetectedIssue = moderationResult.DetectedIssue,
                    ConfidenceScore = moderationResult.ConfidenceScore,
                    Status = ContentStatus.Denied,
                    ReviewedAt = DateTime.UtcNow
                };

                await _unitOfWork
                    .GetRepository<AIModeration, int>()
                    .AddAsync(aiModeration);

                await _unitOfWork.SaveChangesAsync();

                return new CreatePostResultDTO
                {
                    IsCreated = false,
                    Status = ContentStatus.Denied,
                    Message = "Post violates community guidelines."
                };
            }

            return new CreatePostResultDTO
            {
                IsCreated = true,
                Status = ContentStatus.Accepted,
                Message = "Post created successfully."
            };

        }

        public async Task<PagedResult<PostFeedDTO>> GetFeedAsync(string userId, int page, int pageSize)
        {
            // Fetch visible & approved posts
            var spec = new PostFeedSpecification(userId);

            var posts = await _unitOfWork
                .GetRepository<Post, int>()
                .GetAllAsync(spec);

            // Score posts (IN-MEMORY ONLY)
            var scored = posts.Select(post =>
            {
                // Group relation
                GroupRelationType relation =
                    post.Group.GroupMembers.Any(m => m.UserId == userId)
                        ? GroupRelationType.Member
                        : post.Group.GroupFollowers.Any(f => f.UserId == userId)
                            ? GroupRelationType.Follower
                            : GroupRelationType.None;

                // Group priority
                int groupPriority = relation switch
                {
                    GroupRelationType.Member => 50,
                    GroupRelationType.Follower => 25,
                    _ => 5
                };

                // Engagement
                int engagementScore =
                    (post.Likes.Count * 2) +
                    (post.Comments.Count * 3);

                // Time decay
                double ageHours =
                    (DateTime.UtcNow - post.CreatedAt).TotalHours;

                double timePenalty = ageHours * 0.2;

                // Liked penalty
                bool likedByUser =
                    post.Likes.Any(l => l.UserId == userId);

                int likedPenalty = likedByUser ? 15 : 0;

                // Final score
                double feedScore =
                    groupPriority +
                    engagementScore -
                    timePenalty -
                    likedPenalty;

                return new
                {
                    Post = post,
                    Relation = relation,
                    FeedScore = feedScore,
                    IsLikedByUser = likedByUser
                };
            });

            // Order by relevance
            var ordered = scored
                .OrderByDescending(x => x.FeedScore)
                .ThenByDescending(x => x.Post.CreatedAt)
                .ToList();

            // Pagination
            int totalCount = ordered.Count;

            var paged = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map to DTO
            var result = paged.Select(x =>
            {
                var dto = _mapper.Map<PostFeedDTO>(x.Post);
                dto.IsLikedByCurrentUser = x.IsLikedByUser;
                dto.GroupRelation = x.Relation;
                dto.FeedScore = x.FeedScore;
                return dto;
            }).ToList();

            return new PagedResult<PostFeedDTO>
            {
                Items = result,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
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
        public async Task<int> AddCommentAsync(int postId, CreateCommentDTO dto, string userId)
        {
            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = dto.Content
            };

            await _unitOfWork
                .GetRepository<Comment, int>()
                .AddAsync(comment);

            return(await _unitOfWork.SaveChangesAsync());

        }

    }
}
