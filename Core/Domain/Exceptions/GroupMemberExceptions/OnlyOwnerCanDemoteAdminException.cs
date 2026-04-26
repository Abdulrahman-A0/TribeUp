using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public sealed class OnlyOwnerCanDemoteAdminException : ForbiddenException
    {
        public OnlyOwnerCanDemoteAdminException()
            : base("Only the group owner can demote other admins.")
        {
        }
    }
}
