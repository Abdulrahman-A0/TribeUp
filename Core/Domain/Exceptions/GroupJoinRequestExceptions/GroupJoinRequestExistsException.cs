using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupJoinRequestExceptions
{
    public class GroupJoinRequestExistsException : Exception
    {
        public GroupJoinRequestExistsException(int Id) : base($"Request with Id `{Id}` was sent before")
        {
            
        }
    }
}
