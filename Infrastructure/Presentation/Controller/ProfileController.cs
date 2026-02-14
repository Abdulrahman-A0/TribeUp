using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.ProfileModule;
using System.Security.Claims;

namespace Presentation.Controller
{
    [Authorize]
    public class ProfileController(IServiceManager serviceManager) : ApiController
    {
        [HttpGet("Me")]
        public async Task<ActionResult<UserProfileDTO>> GetMyProfileAsync()
            => Ok(await serviceManager.ProfileService.GetMyProfileAsync(UserId));

        [HttpGet("profile-info")]
        public async Task<ActionResult<ProfileSettingsDTO>> GetProfileSettingsAsync()
            => Ok(await serviceManager.ProfileService.GetProfileSettingsAsync(UserId));

        [HttpPut("Name")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO updateProfileDTO)
        {
            await serviceManager.ProfileService
                .UpdateProfileAsync(UserId, updateProfileDTO);

            return NoContent();
        }

        [HttpPut("Avatar")]
        public async Task<IActionResult> UpdateAvatar(UpdateAvatarDTO updateAvatarDTO)
        {
            await serviceManager.ProfileService
                .UpdateAvatarAsync(UserId, updateAvatarDTO);

            return NoContent();
        }

        [Authorize]
        [HttpPut("Picture")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureDTO updateProfilePictureDTO)
        {
            await serviceManager.ProfileService
                .UpdateProfilePictureAsync(UserId, updateProfilePictureDTO);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("Picture/Delete")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            await serviceManager.ProfileService.DeleteProfilePictureAsync(UserId);
            return NoContent();
        }

        [Authorize]
        [HttpPut("Phone")]
        public async Task<IActionResult> UpdatePhone(UpdatePhoneNumberDTO updatePhoneNumberDTO)
        {
            await serviceManager.ProfileService
                .UpdatePhoneNumberAsync(UserId, updatePhoneNumberDTO);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("Phone/Delete")]
        public async Task<IActionResult> DeletePhone()
        {
            await serviceManager.ProfileService
                .DeletePhoneNumberAsync(UserId);

            return NoContent();
        }

    }
}
