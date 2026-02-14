using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.GroupMemberExceptions;
using Microsoft.AspNetCore.Hosting;
using Service.Specifications.GroupMemberSpecs;
using Service.Specifications.GroupSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupModule;
using Shared.DTOs.Posts;
using Shared.Enums;

namespace Service.Implementations
{
    public class GroupService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        IWebHostEnvironment environment, 
        IFileStorageService fileStorage, 
        IUserGroupRelationService _relationService) : IGroupService
    {
        public async Task<List<GroupResultDTO>> GetAllGroupsAsync()
        {
            var repo = unitOfWork.GetRepository<Group, int>();
            var spec = new GroupsWithMembersSpec();

            var groups = await repo.GetAllAsync(spec);

            return mapper.Map<List<GroupResultDTO>>(groups);
        }




        public async Task<List<GroupResultDTO>> GetMyGroupsAsync(string userId)
        {
            var groupRepo = unitOfWork.GetRepository<Group, int>();
            var spec = new GroupsByUserSpec(userId);
            var groups = await groupRepo.GetAllAsync(spec);
            return mapper.Map<List<GroupResultDTO>>(groups);
        }




        public async Task<GroupDetailsResultDTO> GetGroupByIdAsync(int groupId)
        {
            var repo = unitOfWork.GetRepository<Group, int>();
            var spec = new GroupWithMembersSpec(groupId);

            var group = await repo.GetByIdAsync(spec)
                ?? throw new GroupNotFoundException(groupId);
            return mapper.Map<GroupDetailsResultDTO>(group);
        }




        public async Task<GroupResultDTO> CreateGroupAsync(CreateGroupDTO createGroupDTO, string userId)
        {
            var groupRepo = unitOfWork.GetRepository<Group, int>();
            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();

            var group = mapper.Map<Group>(createGroupDTO);

            group.GroupScore = new GroupScore
            {
                TotalPoints = 0,
                LastUpdated = DateTime.UtcNow
            };

            if (createGroupDTO.GroupProfilePicture is not null)
            {
                var picturePath = await fileStorage
                    .SaveAsync(createGroupDTO.GroupProfilePicture, MediaType.GroupProfile);

                group.GroupProfilePicture = picturePath;
            }

            var creatorMember = new GroupMembers
            {
                UserId = userId,
                Role = RoleType.Owner,
                JoinedAt = DateTime.UtcNow
            };

            group.GroupMembers = new List<GroupMembers> { creatorMember };

            await groupRepo.AddAsync(group);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GroupResultDTO>(group);
        }





        public async Task<GroupResultDTO> UpdateGroupAsync(int groupId, UpdateGroupDTO updateGroupDTO, string userId)
        {
            var repo = unitOfWork.GetRepository<Group, int>();

            var group = await repo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            if (!_relationService.IsOwner(groupId) || ! _relationService.IsAdmin(groupId))
                throw new ForbiddenActionException();

            // AutoMapper will only update non-null fields
            mapper.Map(updateGroupDTO, group);

            // No repo.Update(group) needed if EF is tracking
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GroupResultDTO>(group);
        }




        public async Task<bool> DeleteGroupAsync(int groupId, string userId)
        {

            var repo = unitOfWork.GetRepository<Group, int>();
            var group = await repo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            if (!_relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            repo.Delete(group);
            await unitOfWork.SaveChangesAsync();
            return true;
        }




        public async Task<GroupResultDTO> UpdateGroupPictureAsync(int groupId, UpdateGroupPictureDTO updateGroupPictureDTO, string userId)
        {
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            if (!_relationService.IsOwner(groupId) && !_relationService.IsAdmin(groupId))
                throw new ForbiddenActionException();

            var newRelativePath = await fileStorage
                .SaveAsync(updateGroupPictureDTO.Picture, MediaType.GroupProfile);

            if (!string.IsNullOrWhiteSpace(group.GroupProfilePicture))
                await fileStorage.DeleteAsync(group.GroupProfilePicture);

            group.GroupProfilePicture = newRelativePath;

            groupRepo.Update(group);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GroupResultDTO>(group);
        }




        public async Task<bool> DeleteGroupPictureAsync(int groupId, string userId)
        {
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            if (!_relationService.IsOwner(groupId) && !_relationService.IsAdmin(groupId))
                throw new ForbiddenActionException();

            if (string.IsNullOrWhiteSpace(group.GroupProfilePicture))
                return true;

            await fileStorage.DeleteAsync(group.GroupProfilePicture);

            group.GroupProfilePicture = null;

            groupRepo.Update(group);
            await unitOfWork.SaveChangesAsync();

            return true;
        }




        public async Task<PagedResult<GroupResultDTO>> ExploreGroupsAsync(int page, int pageSize, string userId)
        {
            var spec = new ExploreGroupsSpec(page, pageSize);
            var groups = await unitOfWork.GetRepository<Group, int>().GetAllAsync(spec);

            const int memberBonus = 5;
            const int membershipPenalty = 1000;

            var scored =
                groups.Select(group =>
                {
                    int baseScore = group.GroupScore?.TotalPoints ?? 0;
                    int popularityScore = group.GroupMembers.Count * memberBonus;
                    bool isMember = group.GroupMembers.Any(m => m.UserId == userId);


                    int exploreScore = baseScore + popularityScore - (isMember ? membershipPenalty : 0);

                    return new
                    {
                        Group = group,
                        ExploreScore = exploreScore
                    };
                });

            var ordered = scored
               .OrderByDescending(x => x.ExploreScore).ToList();

            return new PagedResult<GroupResultDTO>
            {
                Items = ordered
                .Select(x => mapper.Map<GroupResultDTO>(x.Group))
                .ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = ordered.Count == pageSize
            };
        }

    }
}

