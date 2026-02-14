namespace Shared.DTOs.IdentityModule
{
    public record AuthResponseDTO(string AccessToken, string RefreshToken, UserSummaryDTO UserSummary);

}
