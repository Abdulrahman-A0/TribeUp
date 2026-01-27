using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PostModule
{
    public class CreateEntityResultDTO
    {
        public bool IsCreated { get; set; }
        public ContentStatus Status { get; set; }
        public string Message { get; set; }
    }
}
