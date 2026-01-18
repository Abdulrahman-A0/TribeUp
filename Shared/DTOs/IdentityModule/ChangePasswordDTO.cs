namespace Shared.DTOs.IdentityModule
{
    public record ChangePasswordDTO
    {
        public string CurrentPassword { get; init; } = null!;
        public string NewPassword { get; init; } = null!;
        public string ConfirmNewPassword { get; init; } = null!;
    }

}
