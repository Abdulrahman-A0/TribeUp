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

        public string Password { get; init; }

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; init; }
    }
}
