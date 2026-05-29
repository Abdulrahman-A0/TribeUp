namespace Shared.DTOs.LeaderboardModule;

public class LeaderboardGroupDTO
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = string.Empty;

    public string? GroupProfilePicture { get; set; }

    public int TotalPoints { get; set; }

    public int Rank { get; set; }

    public string BadgeName { get; set; } = string.Empty;

    public string BadgeIcon { get; set; } = string.Empty;
}