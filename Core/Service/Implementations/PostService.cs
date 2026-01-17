using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Posts;
using Service.Specifications;
using Service.Specifications.PostSpecifications;
using ServiceAbstraction.Contracts;
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

        public async Task CreatePostAsync(CreatePostDTO dto, string userId)
        {
            var post = _mapper.Map<Post>(dto);
            post.CreatedAt = DateTime.UtcNow;

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

                // Stop execution and inform the controller
                throw new ValidationException(
                    "Post content violates community guidelines."
                );
            }

        }

        public async Task<IEnumerable<FeedPostDTO>> GetFeedAsync()
        {
            var spec = new PostFeedSpecification();

            var posts = await _unitOfWork
                .GetRepository<Post, int>()
                .GetAllAsync(spec);

            return _mapper.Map<IEnumerable<FeedPostDTO>>(posts);
        }
        public async Task LikePostAsync(int postId, string userId)
        {
            var like = new Like
            {
                PostId = postId,
                UserId = userId
            };

            await _unitOfWork
                .GetRepository<Like, int>()
                .AddAsync(like);

            await _unitOfWork.SaveChangesAsync();

        }
       
        public async Task AddCommentAsync(int postId, CreateCommentDTO dto, string userId)
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

            await _unitOfWork.SaveChangesAsync();

        }



    }
}
