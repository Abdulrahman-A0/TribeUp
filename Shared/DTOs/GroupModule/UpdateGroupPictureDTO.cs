using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupModule
{
    public record UpdateGroupPictureDTO
    {
        public IFormFile Picture { get; init; } = null!;
    }
}
