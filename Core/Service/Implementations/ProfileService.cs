using AutoMapper;
using Domain.Entities.Users;
using Domain.Exceptions.UnAuthorized;
using Domain.Exceptions.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction.Contracts;
using Shared.DTOs.ProfileModule;

namespace Service.Implementations
{
    public class ProfileService(UserManager<ApplicationUser> userManager,
        IMapper mapper, IWebHostEnvironment environment) : IProfileService
    {
        public async Task<UserProfileDTO> GetMyProfileAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UnAuthorizedException();

            return mapper.Map<UserProfileDTO>(user);
        }

        public async Task UpdateProfileAsync(string userId, UpdateProfileDTO updateProfileDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UnAuthorizedException();

            user.FirstName = updateProfileDTO.FirstName;
            user.LastName = updateProfileDTO.LastName;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateAvatarAsync(string userId, UpdateAvatarDTO updateAvatarDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UnAuthorizedException();

            user.Avatar = updateAvatarDTO.Avatar;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateProfilePictureAsync(string userId, UpdateProfilePictureDTO updateProfilePictureDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UnAuthorizedException();

            ValidateImage(updateProfilePictureDTO.Picture);

            var newRelativePath = await SaveProfilePictureAsync(updateProfilePictureDTO.Picture);

            if (!string.IsNullOrWhiteSpace(user.ProfilePicture))
                DeleteFileIfExists(user.ProfilePicture);

            user.ProfilePicture = newRelativePath;

            await userManager.UpdateAsync(user);
        }

        public async Task DeleteProfilePictureAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UnAuthorizedException();

            if (string.IsNullOrWhiteSpace(user.ProfilePicture))
                return;

            DeleteFileIfExists(user.ProfilePicture);

            user.ProfilePicture = null;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdatePhoneNumberAsync(string userId, UpdatePhoneNumberDTO updatePhoneNumberDTO)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UnAuthorizedException();

            user.PhoneNumber = updatePhoneNumberDTO.PhoneNumber;

            await userManager.UpdateAsync(user);
        }

        public async Task DeletePhoneNumberAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UnAuthorizedException();

            user.PhoneNumber = null;

            await userManager.UpdateAsync(user);
        }



        private static void ValidateImage(IFormFile file)
        {
            var allowedTypes = new[] { "image/jpg", "image/jpeg", "image/png", "image/webp" };

            if (!allowedTypes.Contains(file.ContentType))
                throw new ValidationException(["Invalid image type"]);

            if (file.Length > 2 * 1024 * 1024)
                throw new ValidationException(["Image size must be under 2MB"]);
        }

        private async Task<string> SaveProfilePictureAsync(IFormFile file)
        {
            var uploadsRoot = Path.Combine(
                environment.WebRootPath,
                "images",
                "ProfilePictures",
                "Users");

            Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);


            return $"images/ProfilePictures/Users/{fileName}";
        }

        private void DeleteFileIfExists(string relativePath)
        {
            var fullPath = Path.Combine(environment.WebRootPath, relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }


    }
}
