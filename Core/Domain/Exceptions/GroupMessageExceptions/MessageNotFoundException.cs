using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMessageExceptions
{
    public class MessageNotFoundException : NotFoundException
    {
        public MessageNotFoundException(long messageId)
            : base($"Message with ID {messageId} was not found.")
        {
        }
    }
}
