using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PollModule
{
    public class EditPollDTO
    {
        public string? Question { get; set; }
        public List<string>? NewOptions { get; set; }
    }
}
