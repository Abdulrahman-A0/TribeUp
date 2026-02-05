using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupJoinRequestExceptions
{
    public sealed class GroupJoinRequestOwnershipException : ConflictException
    {
        public GroupJoinRequestOwnershipException()
             : base("You are not allowed to modify this join request.")
        {
        }
    }
}
