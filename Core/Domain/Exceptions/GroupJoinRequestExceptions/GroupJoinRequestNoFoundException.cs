using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupJoinRequestExceptions
{
    public class GroupJoinRequestNoFoundException : NotFoundException
    {
        public GroupJoinRequestNoFoundException(int Id) : base($"Request with Id `{Id}` was not found")
        {
        }
    }
}
