using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupInvitationExceptions
{
    public class ActiveInvitationNotFoundException : NotFoundException
    {
        public ActiveInvitationNotFoundException()
            : base($"No active invitation was found.")
        {
        }
    }
}
