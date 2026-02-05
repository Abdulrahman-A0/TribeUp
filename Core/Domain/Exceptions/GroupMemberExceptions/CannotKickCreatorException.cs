using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public sealed class CannotKickCreatorException : ConflictException
    {
        public CannotKickCreatorException()
            : base("The group creator cannot be removed from the group.")
        {
        }
    }
}
