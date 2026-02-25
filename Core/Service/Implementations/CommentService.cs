using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Entities.Posts;
using Domain.Entities.Users;
using Domain.Exceptions;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.PostExceptions;
using Domain.Exceptions.ValidationExceptions;
using Microsoft.AspNetCore.Identity;
using Service.Specifications.CommentSpecification;
using Service.Specifications.ModerationSpecifications;
using Service.Specifications.PostSpecifications;
using ServiceAbstraction.Contracts;
using Shared.DTOs.NotificationModule;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IGroupScoreService _groupScoreService;
        private readonly INotificationService _notificationService;
        private readonly IAIModerationManager _aiModerationManager;
        private readonly IUserGroupRelationService _relationService;

        private readonly IGenericRepository<Tag, int> tagRepo;
        private readonly IGenericRepository<Post, int> postRepo;
        private readonly IGenericRepository<Like, int> likeRepo;
        private readonly IGenericRepository<Group, int> groupRepo;
        private readonly IGenericRepository<Comment, int> commentRepo;
        private readonly IGenericRepository<AIModeration, int> moderationRepo;

        UserManager<ApplicationUser> _userManager;

        private readonly int PostPoints = 5;
        private readonly int CommentPoints = 1;
        public CommentService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IGroupScoreService groupScoreService,
            INotificationService notificationService,
            IAIModerationManager aiModerationManager,
            IUserGroupRelationService relationService,
            UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _groupScoreService = groupScoreService;
            _notificationService = notificationService;
            _aiModerationManager = aiModerationManager;
            _relationService = relationService;

            _userManager = userManager;

            tagRepo = _unitOfWork.GetRepository<Tag, int>();
            postRepo = _unitOfWork.GetRepository<Post, int>();
            likeRepo = _unitOfWork.GetRepository<Like, int>();
            groupRepo = _unitOfWork.GetRepository<Group, int>();
            commentRepo = _unitOfWork.GetRepository<Comment, int>();
            moderationRepo = _unitOfWork.GetRepository<AIModeration, int>();
        }
        public async Task<ToggleLikeDTO> CommentToggleLikeAsync(
            string userId, 
            string username, 
            int commentId)
        {
            var comment = await commentRepo.GetByIdAsync(commentId)
                           ?? throw new CommentNotFoundException(commentId);

            var spec = new LikeByCommentIdAbdUserIdSpecifications(userId, commentId);
            var existingLike = await likeRepo.GetByIdAsync(spec);

            if (existingLike is null)
            {
                var like = new Like
                {
                    CommentId = commentId,
                    UserId = userId
                };

                await likeRepo.AddAsync(like);
                await _unitOfWork.SaveChangesAsync();

                await _notificationService.CreateAsync(new CreateNotificationDTO
                {
                    RecipientUserId = comment.UserId,
                    ActorUserId = userId,
                    Type = NotificationType.PostLike,
                    Title = $"New like on your comment",
                    Message = $"{username} liked your post: \"{GetFirstWord(comment.Content)}\"",
                    ReferenceId = comment.Id
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


        public async Task<PagedResult<LikesResultDTO>> GetLikesByCommentIdAsync(
         int commentId,
         int page,
         int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new PageIndexAndPageSizeValidationException(page, pageSize);

            var comment = await commentRepo.GetByIdAsync(new CommentByIdSpecification(commentId))
                 ?? throw new CommentNotFoundException(commentId);

            if (!_relationService.IsMember(comment.Post.GroupId) && comment.Post.Accessibility == AccessibilityType.Private)
                throw new ForbiddenActionException();

            var spec = new LikesByCommentIdSpecification(commentId, page, pageSize);
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
            await _userManager.FindByIdAsync(userId);

            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new PostAndCommentContentValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["CommentContent"] = new[] { "Comment must contain text." }
                    });

            var post = await postRepo.GetByIdAsync(postId)
                ?? throw new PostNotFoundException(postId);

            if (!_relationService.IsMember(post.GroupId) && post.Accessibility == AccessibilityType.Private)
                throw new ForbiddenActionException();

            var comment = _mapper.Map<Comment>(dto);
            comment.Content = comment.Content.Trim();
            comment.UserId = userId;
            comment.PostId = postId;


            await commentRepo.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            var mappedComment = _mapper.Map<CommentResultDTO>(comment);

            # region AI moderation result

            var moderationResult = await _aiModerationManager.ModerateAsync(
                comment.Content ?? string.Empty,
                ModeratedEntityType.Comment,
                comment.Id);

            if (!moderationResult.IsAccepted)
            {
                return new CreateEntityResultDTO
                {
                    IsCreated = true,
                    Status = ContentStatus.Denied,
                    Message = "Comment violates community guidelines.",
                    Comment = mappedComment
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
            
            foreach (var taggedUserId in dto.TaggedUserIds.Distinct())
            {
                var tag = new Tag
                {
                    CommentId = comment.Id,
                    UserId = taggedUserId
                };
                await tagRepo.AddAsync(tag);

                await _notificationService.CreateAsync(new CreateNotificationDTO
                {
                    RecipientUserId = taggedUserId,
                    ActorUserId = userId,
                    Type = NotificationType.PostTag,
                    Title = "You were tagged in a comment",
                    Message = $"{username} tagged you in his comment: \"{GetFirstWord(comment.Content)}\"",
                    ReferenceId = comment.Id
                });
            }
            await _groupScoreService.IncreaseOnActionAsync(post.GroupId, CommentPoints);
            await _unitOfWork.SaveChangesAsync();

            return new CreateEntityResultDTO
            {
                IsCreated = true,
                Status = ContentStatus.Accepted,
                Message = "Comment created successfully",
                Comment = mappedComment

            };

        }

        public async Task<CreateEntityResultDTO> EditCommentAsync(
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

            var tags = tagRepo
                .AsQueryable()
                .Where(t => t.CommentId == commentId)
                .ToList();
            foreach (var tag in tags)
            {
                tagRepo.Delete(tag);
            }

            foreach (var taggedUserId in dto.TaggedUserIds.Distinct())
            {
                var tag = new Tag
                {
                    CommentId = comment.Id,
                    UserId = taggedUserId
                };
                await tagRepo.AddAsync(tag);

                await _notificationService.CreateAsync(new CreateNotificationDTO
                {
                    RecipientUserId = taggedUserId,
                    ActorUserId = userId,
                    Type = NotificationType.PostTag,
                    Title = "You were tagged in a comment",
                    Message = $"{username} tagged you in his comment: \"{GetFirstWord(comment.Content)}\"",
                    ReferenceId = comment.Id
                });
            }

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

            if (!_relationService.IsMember(post.GroupId) && post.Accessibility == AccessibilityType.Private)
                throw new ForbiddenActionException();

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

            foreach (var dto in mapped)
            {
                var comment = comments.First(c => c.Id == dto.Id);
                dto.IsLikedByCurrentUser =
                    comment.Likes.Any(l => l.UserId == userId);
            }

            return new PagedResult<CommentResultDTO>
            {
                Items = mapped,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = totalCount > page * pageSize
            };

        }


        public async Task<DeleteEntityResultDTO> DeleteCommentAsync(
            string userId,
            int commentId)
        {

            var spec = new CommentByIdSpecification(commentId);
            var comment = await commentRepo.GetByIdAsync(spec)
                ?? throw new CommentNotFoundException(commentId);

            var post = await postRepo.GetByIdAsync(comment.PostId)
                ?? throw new PostNotFoundException(comment.PostId);

            if (!_relationService.IsAdmin(post.GroupId) && !_relationService.IsOwner(post.GroupId))
                throw new ForbiddenActionException();


            await _groupScoreService.DecreaseOnActionAsync(post.GroupId, CommentPoints);

            var likes = likeRepo
                .AsQueryable()
                .Where(l => l.CommentId == commentId)
                .ToList();
            foreach (var like in likes)
            {
                likeRepo.Delete(like);
            }
            var tags = tagRepo
                .AsQueryable()
                .Where(t => t.CommentId == commentId)
                .ToList();
            foreach (var tag in tags)
            {
                tagRepo.Delete(tag);
            }

            commentRepo.Delete(comment);
            await _unitOfWork.SaveChangesAsync();
            return new DeleteEntityResultDTO
            {
                Message = "Deleted successfully"
            };

        }



        #region Private Methods
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
