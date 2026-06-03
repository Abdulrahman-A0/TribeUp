using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PollModule
{
    public class CreatePollDTO
    {
        public string Question { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<string> Options { get; set; }
        public bool AllowMultipleAnswers { get; set; }
    }
}
