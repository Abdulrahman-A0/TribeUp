using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupModule
{
    public record GroupResultDTO
    {
        public int Id { get; init; }
        public string GroupName { get; init; }
        public string Description { get; init; }
        public string GroupProfilePicture { get; init; }
        public DateTime CreatedAt { get; init; }
        public AccessibilityType Accessibility { get; init; }
        public int MembersCount { get; init; }
    }
}
