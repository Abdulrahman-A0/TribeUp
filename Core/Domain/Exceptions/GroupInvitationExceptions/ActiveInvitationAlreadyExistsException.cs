using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupInvitationExceptions
{
    public sealed class ActiveInvitationAlreadyExistsException : ConflictException
    {
        public ActiveInvitationAlreadyExistsException()
            : base("An active invitation already exists for this group.")
        {
        }
    }
}
