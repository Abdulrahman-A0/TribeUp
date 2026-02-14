using Shared.DTOs.ProfileModule;

namespace ServiceAbstraction.Contracts
{
    public interface IProfileService
    {
        Task<UserProfileDTO> GetMyProfileAsync(string userId);
        Task<ProfileSettingsDTO> GetProfileSettingsAsync(string userId);
        Task UpdateProfileAsync(string userId, UpdateProfileDTO updateProfileDTO);
        Task UpdateAvatarAsync(string userId, UpdateAvatarDTO updateAvatarDTO);
        Task UpdateProfilePictureAsync(string userId, UpdateProfilePictureDTO updateProfilePictureDTO);
        Task DeleteProfilePictureAsync(string userId);
        Task UpdatePhoneNumberAsync(string userId, UpdatePhoneNumberDTO updatePhoneNumberDTO);
        Task DeletePhoneNumberAsync(string userId);
    }
}
