using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.IdentityModule
{
    public record RegisterDTO
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string UserName { get; init; }

        [EmailAddress]
        public string Email { get; init; }

        [Phone]
        public string? PhoneNumber { get; init; }
        public string? ProfilePicture { get; init; }
        public string? Avatar { get; init; }

        public string Password { get; init; }

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; init; }
    }
}
