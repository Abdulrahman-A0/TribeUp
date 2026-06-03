using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PollModule
{
    public class PollResultDTO
    {
        public int PollId { get; set; }
        public string Question { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string CreatedByUserName { get; set; }
        public int TotalUniqueVoters { get; set; }
        public bool IsExpired { get; set; }
        public bool AllowMultipleAnswers { get; set; }
        public List<PollOptionResultDTO> Options { get; set; }
    }
}
