namespace Shared.DTOs.ProfileModule
{
    public record ProfileSettingsDTO
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string UserName { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Bio { get; init; }
        public string ProfilePicture { get; init; }
        public string? CoverPicture { get; init; }

    }
}
