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
            : base("This user has already created an active invitation.")
        {
        }
    }
}
