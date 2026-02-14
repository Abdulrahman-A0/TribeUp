namespace Shared.DTOs.IdentityModule
{
    public record UserSummaryDTO
    {
        public string Id { get; init; }
        public string UserName { get; init; }
        public string FullName { get; init; }
        public string? ProfilePicture { get; init; }
    }
}
