namespace Shared.DTOs.IdentityModule
{
    public record ResetPasswordDTO
    {
        public string Email { get; init; } = null!;
        public string Token { get; init; } = null!;
        public string NewPassword { get; init; } = null!;
        public string ConfirmPassword { get; init; } = null!;
    }

}
