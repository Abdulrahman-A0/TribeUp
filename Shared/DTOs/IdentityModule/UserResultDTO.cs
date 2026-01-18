using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.IdentityModule
{
    public record UserResultDTO
    {
        public string FullName { get; init; }
        public string Email { get; init; }
        public string? ProfilePicture { get; init; }
        public string? Avatar { get; init; }
        public string Token { get; init; }
    }
}
