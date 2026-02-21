using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupInvitationExceptions
{
    public class InvitationNotFoundException : NotFoundException
    {
        public InvitationNotFoundException(int invitationId) : base($"Invitation with id {invitationId} was not found.")
        {
        }
    }
}
