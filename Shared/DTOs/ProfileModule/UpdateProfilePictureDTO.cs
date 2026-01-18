using Microsoft.AspNetCore.Http;

namespace Shared.DTOs.ProfileModule
{
    public class UpdateProfilePictureDTO
    {
        public IFormFile Picture { get; init; } = null!;
    }
}
