using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.PostExceptions
{
    public class CommentNotFoundException:NotFoundException
    {
        public CommentNotFoundException(int Id) :base($"Comment With Id '{Id}' Was Not Found!")
        {
            
        }
    }
}
