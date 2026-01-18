namespace Shared.DTOs.ProfileModule
{
    public record UserProfileDTO()
    {
        public string FullName { get; init; }
        public string Email { get; init; }
        public string ProfilePicture { get; init; }
        public string? Avatar { get; init; }
    }
}
