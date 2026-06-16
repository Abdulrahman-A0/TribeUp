using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PollModule
{
    public class VoterDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime VotedAt { get; set; }
    }
}
