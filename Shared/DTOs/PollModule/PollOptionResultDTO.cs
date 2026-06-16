using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PollModule
{
    public class PollOptionResultDTO
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; }
        public int VotesCount { get; set; }
        public double Percentage { get; set; }
        public bool IsVotedByCurrentUser { get; set; }
        public List<VoterDTO> Voters { get; set; }
    }
}
