using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.IdentityModule
{
    public record LoginDTO
    {
        [EmailAddress]
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
