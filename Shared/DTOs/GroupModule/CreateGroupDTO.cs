using Microsoft.AspNetCore.Http;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupModule
{
    public record CreateGroupDTO
    {
        [Required(ErrorMessage = "Group name is required")]
        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters")]
        public string GroupName { get; init; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; init; }

        public AccessibilityType Accessibility { get; init; } = AccessibilityType.Public;
        public IFormFile? GroupProfilePicture { get; init; }
    }
}
