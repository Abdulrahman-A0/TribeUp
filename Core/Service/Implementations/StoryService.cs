using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Stories;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.StoryExceptions;
using Domain.Exceptions.ValidationExceptions;
using Microsoft.EntityFrameworkCore;
using Service.Specifications.StorySpecifications;
using ServiceAbstraction.Contracts;
using Shared.DTOs.StoriesModule;
using Shared.Enums;

namespace Service.Implementations
{
    public class StoryService(IUnitOfWork unitOfWork, IMapper mapper,
        IFileStorageService fileStorage, IUserGroupRelationService userGroupRelation) : IStoryService
    {
        public async Task<StoryResponseDTO> CreateStoryAsync(CreateStoryDTO dto, string currentUserId)
        {
            if (!userGroupRelation.IsMember(dto.GroupId))
                throw new ForbiddenActionException();

            if (string.IsNullOrWhiteSpace(dto.Caption) && dto.MediaFile is null)
                throw new StoryContentValidationException();

            string? mediaPath = null;
            if (dto.MediaFile != null)
                mediaPath = await fileStorage.SaveAsync(dto.MediaFile, MediaType.StoryMedia);

            var story = mapper.Map<Story>(dto);
            story.UserId = currentUserId;
            story.MediaURL = mediaPath;

            await unitOfWork.GetRepository<Story, int>().AddAsync(story);
            await unitOfWork.SaveChangesAsync();

            var storyWithUser = await unitOfWork.GetRepository<Story, int>()
                .GetByIdAsync(new StoryWithUserByIdSpecification(story.Id));

            var response = mapper.Map<StoryResponseDTO>(storyWithUser) with
            {
                IsViewedByCurrentUser = false
            };

            return response;
        }

        public async Task DeleteStoryAsync(int storyId, string currentUserId)
        {
            var storyRepo = unitOfWork.GetRepository<Story, int>();

            var story = await storyRepo.GetByIdAsync(storyId)
                ?? throw new StoryNotFoundException(storyId);

            if (story.UserId != currentUserId && !userGroupRelation.IsAdmin(story.GroupId))
                throw new ForbiddenActionException();

            if (!string.IsNullOrEmpty(story.MediaURL))
                await fileStorage.DeleteAsync(story.MediaURL);

            storyRepo.Delete(story);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<StoryResponseDTO>> GetActiveGroupStoriesAsync(int groupId, string currentUserId)
        {
            bool isMember = userGroupRelation.IsMember(groupId);

            var spec = new ActiveGroupStoriesSpecification(groupId, isMember);

            var stories = (await unitOfWork.GetRepository<Story, int>().GetAllAsync(spec)).ToList();

            var responses = stories
                .Select(s => mapper.Map<StoryResponseDTO>(s) with
                {
                    IsViewedByCurrentUser = s.StoryViews.Any(v => v.UserId == currentUserId),
                    ViewsCount = s.ViewsCount
                })
                .ToList();

            return responses;
        }

        public async Task<IEnumerable<StoryFeedItemDTO>> GetStoryFeedAsync(string currentUserId)
        {
            var allowedGroupIds = userGroupRelation
                .GetAccessibleGroupIds()
                .ToList();

            if (!allowedGroupIds.Any())
                return Enumerable.Empty<StoryFeedItemDTO>();

            var query = unitOfWork.GetRepository<Story, int>().AsQueryable()

                // Filter out expired stories and unauthorized groups
                .Where(s => allowedGroupIds.Contains(s.GroupId) && s.ExpiresAt > DateTime.UtcNow)

                .GroupBy(s => new { s.GroupId, s.Group.GroupName, s.Group.GroupProfilePicture })

                // 4. Project directly to the DTO (Skipping AutoMapper for safety and performance)
                .Select(g => new StoryFeedItemDTO
                {
                    GroupId = g.Key.GroupId,
                    GroupName = g.Key.GroupName,
                    GroupProfilePicture = g.Key.GroupProfilePicture,
                    HasUnseenStories = g.Any(story => !story.StoryViews.Any(v => v.UserId == currentUserId)),
                    LatestStoryDate = g.Max(story => story.CreatedAt)

                })
                .OrderByDescending(f => f.HasUnseenStories)
                .ThenByDescending(f => f.LatestStoryDate);

            return await query.ToListAsync();
        }

        public async Task MarkStoryAsViewedAsync(int storyId, string currentUserId)
        {
            // 1. Load story with its views and user; the spec ensures the story is active (not expired)
            var story = await unitOfWork.GetRepository<Story, int>()
                .GetByIdAsync(new StoryWithViewsByIdSpecification(storyId))
                ?? throw new StoryNotFoundException(storyId);

            // 2. Authorization: member or public
            if (!userGroupRelation.IsMember(story.GroupId) && story.Accessibility != AccessibilityType.Public)
                throw new ForbiddenActionException();

            // 3. If story is expired the specification would have filtered it out;
            //    but handle defensively in case ExpiresAt became <= now after load
            if (story.ExpiresAt.HasValue && story.ExpiresAt.Value <= DateTime.UtcNow)
                throw new StoryNotFoundException(story.Id);

            // 4. If already viewed by this user do nothing
            var alreadyViewed = story.StoryViews.Any(v => v.UserId == currentUserId);
            if (alreadyViewed)
                return;

            // 5. Add a StoryView and increment views count
            var storyView = new StoryView
            {
                StoryId = story.Id,
                UserId = currentUserId,
                ViewedAt = DateTime.UtcNow
            };

            await unitOfWork.GetRepository<StoryView, int>().AddAsync(storyView);

            // increment views count on story entity and persist
            story.ViewsCount += 1;

            unitOfWork.GetRepository<Story, int>().Update(story);

            await unitOfWork.SaveChangesAsync();
        }
    }
}
