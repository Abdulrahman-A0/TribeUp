using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupJoinRequestExceptions
{
    public class InvalidGroupJoinRequestException : Exception
    {
        public InvalidGroupJoinRequestException(int groupId, string accessibility)
            : base($"Group with ID {groupId} is {accessibility}. Join requests are only allowed for private groups.")
        {
        }
    }
}
