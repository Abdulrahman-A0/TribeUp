namespace Shared.DTOs.ProfileModule
{
    public record UserProfileHeaderDTO
    {
        public string Id { get; init; }
        public string FullName { get; init; }
        public string UserName { get; init; }
        public string? ProfilePicture { get; init; }
        public string? CoverPicture { get; init; }
        public string? Bio { get; init; }
        public DateTime CreatedAt { get; init; }

        public int TribesCount { get; init; }
        public int PostsCount { get; init; }

        public bool IsOwnProfile { get; set; }

    }
}
