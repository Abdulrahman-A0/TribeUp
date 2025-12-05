namespace Shared.DTOs.IdentityModule
{
    public record UserResultDTO(string FullName, string Email, string? ProfilePicture, string Token);
}
