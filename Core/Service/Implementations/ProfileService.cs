using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Entities.Posts;
using Domain.Entities.Users;
using Domain.Exceptions.UserExceptions;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction.Contracts;
using Shared.DTOs.ProfileModule;
using Shared.Enums;

namespace Service.Implementations
{
    public class ProfileService(UserManager<ApplicationUser> userManager,
        IMapper mapper, IFileStorageService fileStorage, IUnitOfWork unitOfWork) : IProfileService
    {
        public async Task<UserProfileDTO> GetMyProfileAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            return mapper.Map<UserProfileDTO>(user);
        }
        public async Task<UserProfileHeaderDTO> GetUserProfileHeaderAsync(string targetUserName, string currentUserId)
        {
            var targetUser = await userManager.FindByNameAsync(targetUserName)
                ?? throw new UserNotFoundException(targetUserName);

            var profileDto = mapper.Map<UserProfileHeaderDTO>(targetUser);

            var postsCount = await unitOfWork
                .GetRepository<Post, int>()
                .CountAsync(p => p.UserId == targetUser.Id);

            var tribesCount = await unitOfWork
                .GetRepository<GroupMembers, int>()
                .CountAsync(gm => gm.UserId == targetUser.Id);

            return profileDto with
            {
                PostsCount = postsCount,
                TribesCount = tribesCount,
                IsOwnProfile = (targetUser.Id == currentUserId)
            };
        }

        public async Task<ProfileSettingsDTO> GetProfileSettingsAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) throw new UserNotFoundException(userId);

            return mapper.Map<ProfileSettingsDTO>(user);
        }

        public async Task UpdateProfileAsync(string userId, UpdateProfileDTO updateProfileDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            user.FirstName = updateProfileDTO.FirstName;
            user.LastName = updateProfileDTO.LastName;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateAvatarAsync(string userId, UpdateAvatarDTO updateAvatarDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            user.Avatar = updateAvatarDTO.Avatar;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateProfilePictureAsync(string userId, UpdateProfilePictureDTO updateProfilePictureDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            var newRelativePath = await fileStorage
                .SaveAsync(updateProfilePictureDTO.Picture, MediaType.UserProfile);


            if (!string.IsNullOrWhiteSpace(user.ProfilePicture))
                await fileStorage.DeleteAsync(user.ProfilePicture);

            user.ProfilePicture = newRelativePath;

            await userManager.UpdateAsync(user);
        }

        public async Task DeleteProfilePictureAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            if (string.IsNullOrWhiteSpace(user.ProfilePicture))
                return;

            await fileStorage.DeleteAsync(user.ProfilePicture);

            user.ProfilePicture = null;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdatePhoneNumberAsync(string userId, UpdatePhoneNumberDTO updatePhoneNumberDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            user.PhoneNumber = updatePhoneNumberDTO.PhoneNumber;

            await userManager.UpdateAsync(user);
        }

        public async Task DeletePhoneNumberAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            user.PhoneNumber = null;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateBioAsync(string userId, UpdateBioDTO updateBioDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            user.Bio = updateBioDTO.Bio;

            await userManager.UpdateAsync(user);
        }

        public async Task DeleteBioAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            user.Bio = null;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateCoverPictureAsync(string userId, UpdateCoverPictureDTO updateCoverPictureDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            var newRelativePath = await fileStorage
                .SaveAsync(updateCoverPictureDTO.CoverPicture, MediaType.UserCover);

            if (!string.IsNullOrEmpty(user.CoverPicture))
                await fileStorage.DeleteAsync(user.CoverPicture);

            user.CoverPicture = newRelativePath;

            await userManager.UpdateAsync(user);
        }

        public async Task DeleteCoverPictureAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            if (string.IsNullOrWhiteSpace(user.CoverPicture))
                return;

            await fileStorage.DeleteAsync(user.CoverPicture);

            user.CoverPicture = null;

            await userManager.UpdateAsync(user);
        }

    }
}
