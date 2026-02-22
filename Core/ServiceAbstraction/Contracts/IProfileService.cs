using Shared.DTOs.ProfileModule;

namespace ServiceAbstraction.Contracts
{
    public interface IProfileService
    {
        Task<UserProfileDTO> GetMyProfileAsync(string userId);
        Task<UserProfileHeaderDTO> GetUserProfileHeaderAsync(string targetUserName, string currentUserId);
        Task<ProfileSettingsDTO> GetProfileSettingsAsync(string userId);
        Task UpdateProfileAsync(string userId, UpdateProfileDTO updateProfileDTO);
        Task UpdateAvatarAsync(string userId, UpdateAvatarDTO updateAvatarDTO);
        Task UpdateProfilePictureAsync(string userId, UpdateProfilePictureDTO updateProfilePictureDTO);
        Task DeleteProfilePictureAsync(string userId);
        Task UpdatePhoneNumberAsync(string userId, UpdatePhoneNumberDTO updatePhoneNumberDTO);
        Task DeletePhoneNumberAsync(string userId);
        Task UpdateBioAsync(string userId, UpdateBioDTO updateBioDTO);
        Task DeleteBioAsync(string userId);
        Task UpdateCoverPictureAsync(string userId, UpdateCoverPictureDTO updateCoverPictureDTO);
        Task DeleteCoverPictureAsync(string userId);
    }
}
