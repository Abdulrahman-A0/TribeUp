using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public class GroupMemberNotFoundException : NotFoundException
    {
        public GroupMemberNotFoundException(string UserId) : base($"Member with Id `{UserId}` was not found")
        {
        }
    }
}
