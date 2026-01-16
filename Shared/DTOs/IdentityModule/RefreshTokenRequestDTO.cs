namespace Shared.DTOs.IdentityModule
{
    public record RefreshTokenRequestDTO
    {
        public string RefreshToken { get; init; } = null!;
        public string DeviceId { get; init; } = null!;
    }

}
