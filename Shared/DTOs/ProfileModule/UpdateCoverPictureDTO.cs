using Microsoft.AspNetCore.Http;

namespace Shared.DTOs.ProfileModule
{
    public record UpdateCoverPictureDTO
    {
        public IFormFile CoverPicture {  get; init; } = null!;
    }
}
