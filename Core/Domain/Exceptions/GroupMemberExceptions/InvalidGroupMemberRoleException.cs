using Domain.Exceptions.Abstraction;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public sealed class InvalidGroupMemberRoleException : ValidationException
    {
        public InvalidGroupMemberRoleException(RoleType actualRole)
            : base(new Dictionary<string, string[]>
            {
                {
                    "Role",
                    new[] { $"Invalid role for this action. Current role is '{actualRole}'." }
                }
            })
        {
        }
    }
}
