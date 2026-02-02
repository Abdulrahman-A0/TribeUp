using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public sealed class CannotDemoteSelfException : ConflictException
    {
        public CannotDemoteSelfException()
            : base("You cannot demote yourself from the admin role.")
        {
        }
    }
}
