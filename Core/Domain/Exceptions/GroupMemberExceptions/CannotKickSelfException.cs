using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public sealed class CannotKickSelfException : ConflictException
    {
        public CannotKickSelfException()
            : base("You cannot remove yourself from the group.")
        {
        }
    }
}
