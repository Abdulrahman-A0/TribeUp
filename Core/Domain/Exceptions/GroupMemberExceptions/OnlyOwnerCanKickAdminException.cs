using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public sealed class OnlyOwnerCanKickAdminException : ForbiddenException
    {
        public OnlyOwnerCanKickAdminException()
            : base("Only the group owner has the authority to remove other admins.")
        {
        }
    }
}
