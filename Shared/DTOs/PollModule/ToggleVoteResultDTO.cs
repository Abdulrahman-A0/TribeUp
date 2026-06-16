using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PollModule
{
    public class ToggleVoteResultDTO
    {
        public string Message { get; set; }
        public bool IsVoted { get; set; }
    }
}
