using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupJoinRequestExceptions
{
    public sealed class GroupJoinRequestNotAllowedException
        : ConflictException
    {
        public GroupJoinRequestNotAllowedException(string reason)
            : base(reason)
        {
        }
    }
}
