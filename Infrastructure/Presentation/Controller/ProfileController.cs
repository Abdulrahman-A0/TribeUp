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
            => Ok(await serviceManager.ProfileService.GetMyProfileAsync(GetCurrentUserId()));

        [HttpPut("Name")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO updateProfileDTO)
        {
            var userId = GetCurrentUserId();

            await serviceManager.ProfileService
                .UpdateProfileAsync(userId, updateProfileDTO);

            return NoContent();
        }

        [HttpPut("Avatar")]
        public async Task<IActionResult> UpdateAvatar(UpdateAvatarDTO updateAvatarDTO)
        {
            var userId = GetCurrentUserId();

            await serviceManager.ProfileService
                .UpdateAvatarAsync(userId, updateAvatarDTO);

            return NoContent();
        }

        [Authorize]
        [HttpPut("Picture")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureDTO updateProfilePictureDTO)
        {
            await serviceManager.ProfileService
                .UpdateProfilePictureAsync(GetCurrentUserId(), updateProfilePictureDTO);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("Picture/Delete")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            await serviceManager.ProfileService.DeleteProfilePictureAsync(GetCurrentUserId());
            return NoContent();
        }

        [Authorize]
        [HttpPut("Phone")]
        public async Task<IActionResult> UpdatePhone(UpdatePhoneNumberDTO updatePhoneNumberDTO)
        {
            await serviceManager.ProfileService
                .UpdatePhoneNumberAsync(GetCurrentUserId(), updatePhoneNumberDTO);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("Phone/Delete")]
        public async Task<IActionResult> DeletePhone()
        {
            await serviceManager.ProfileService
                .DeletePhoneNumberAsync(GetCurrentUserId());

            return NoContent();
        }

    }
}
