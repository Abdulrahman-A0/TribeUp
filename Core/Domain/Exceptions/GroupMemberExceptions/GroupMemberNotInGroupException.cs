using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public class GroupMemberNotInGroupException : ValidationException
    {
        public GroupMemberNotInGroupException()
            : base(new Dictionary<string, string[]>
            {
            { "GroupMember", new[] { "This member does not belong to the specified group." } }
            })
        {
        }
    }
}
