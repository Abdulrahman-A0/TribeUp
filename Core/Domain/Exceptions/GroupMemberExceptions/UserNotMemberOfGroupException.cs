using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public class UserNotMemberOfGroupException : NotFoundException
    {
        public UserNotMemberOfGroupException()
            : base("User is not a member of this group.")
        {
        }
    }
}
