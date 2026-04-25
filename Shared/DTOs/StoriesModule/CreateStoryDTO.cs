using Microsoft.AspNetCore.Http;
using Shared.Enums;

namespace Shared.DTOs.StoriesModule
{
    public record CreateStoryDTO
    {
        public int GroupId { get; init; }
        public string? Caption { get; init; }
        public AccessibilityType Accessibility { get; init; } = AccessibilityType.Public;
        public IFormFile? MediaFile { get; init; }
    }
}
