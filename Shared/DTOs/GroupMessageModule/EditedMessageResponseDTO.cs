using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupMessageModule
{
    public class EditedMessageResponseDTO
    {
        public long Id { get; set; }
        public int GroupId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsEdited { get; set; } = true;
    }
}
