using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.IdentityModule
{
    public class ForgotPasswordDTO
    {
        [EmailAddress]
        public string Email { get; init; } = null!;
    }
}
