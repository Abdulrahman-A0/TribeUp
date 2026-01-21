using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.GroupMemberExceptions;
using Domain.Exceptions.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Service.Specifications.GroupMemberSpecs;
using Service.Specifications.GroupSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMemberModule;
using Shared.DTOs.GroupModule;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class GroupService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment environment) : IGroupService
    {
        public async Task<List<GroupResultDTO>> GetAllGroupsAsync()
        {
            var repo = unitOfWork.GetRepository<Group, int>();
            var groups = await repo.GetAllAsync(asNoTracking: true);
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
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();

            var group = mapper.Map<Group>(createGroupDTO);

            await groupRepo.AddAsync(group);

            var adminMember = new GroupMember
            {
                Group = group,
                UserId = userId,
                Role = RoleType.Admin,
                JoinedAt = DateTime.UtcNow
            };

            await memberRepo.AddAsync(adminMember);

            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GroupResultDTO>(group);
        }




        public async Task<GroupResultDTO> UpdateGroupAsync(int groupId, UpdateGroupDTO updateGroupDTO, string userId)
        {

            var repo = unitOfWork.GetRepository<Group, int>();
            var group = await repo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            await EnsureUserIsAdmin(groupId, userId);

            mapper.Map(updateGroupDTO, group);

            repo.Update(group);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GroupResultDTO>(group);
        }

        


        public async Task<bool> DeleteGroupAsync(int groupId, string userId)
        {

            var repo = unitOfWork.GetRepository<Group, int>();
            var group = await repo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            await EnsureUserIsAdmin(groupId, userId);

            repo.Delete(group);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
        



        public async Task<GroupResultDTO> UpdateGroupPictureAsync(int groupId, UpdateGroupPictureDTO updateGroupPictureDTO, string userId)
        {
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            await EnsureUserIsAdmin(groupId, userId);

            ValidateImage(updateGroupPictureDTO.Picture);

            var newRelativePath = await SaveGroupPictureAsync(updateGroupPictureDTO.Picture);

            if(!string.IsNullOrWhiteSpace(group.GroupProfilePicture))
                DeleteFileIfExists(group.GroupProfilePicture);

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

            await EnsureUserIsAdmin(groupId, userId);

            if(string.IsNullOrWhiteSpace(group.GroupProfilePicture))
                return true;

            DeleteFileIfExists(group.GroupProfilePicture);

            group.GroupProfilePicture = null;

            groupRepo.Update(group);
            await unitOfWork.SaveChangesAsync();

            return true;
        }





        private async Task<GroupMember> EnsureUserIsAdmin(int groupId, string userId)
        {
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();

            var spec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var member = await memberRepo.GetByIdAsync(spec)
                ?? throw new GroupMemberNotFoundException(userId);

            if (member.Role != RoleType.Admin)
                throw new UnauthorizedAccessException("Only group admins can perform this action");

            return member;
        }

        private static void ValidateImage(IFormFile file)
        {
            var allowedTypes = new[] { "image/jpg", "image/jpeg", "image/png", "image/webp" };

            if (!allowedTypes.Contains(file.ContentType))
                throw new ValidationException(["Invalid image type"]);

            if (file.Length > 2 * 1024 * 1024)
                throw new ValidationException(["Image size must be under 2MB"]);
        }

        private async Task<string> SaveGroupPictureAsync(IFormFile file)
        {
            var uploadsRoot = Path.Combine(
                environment.WebRootPath,
                "images",
                "ProfilePictures",
                "Groups");

            Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);


            return $"images/ProfilePictures/Groups/{fileName}";
        }

        private void DeleteFileIfExists(string relativePath)
        {
            var fullPath = Path.Combine(environment.WebRootPath, relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}

