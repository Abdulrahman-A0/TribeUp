using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public sealed class CannotRemoveLastAdminException : ConflictException
    {
        public CannotRemoveLastAdminException()
            : base("You cannot remove the last admin from the group.")
        {
        }
    }
}
