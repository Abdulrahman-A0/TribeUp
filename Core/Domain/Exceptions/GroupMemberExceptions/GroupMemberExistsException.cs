using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public class GroupMemberExistsException : Exception
    {
        public GroupMemberExistsException(string UserId) : base($"Member with Id `{UserId}` exists in this group")
        {
            
        }
    }
}
