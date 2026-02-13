using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupInvitationModule
{
    public class CreateInvitationDTO
    {
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUses { get; set; }

    }

}
