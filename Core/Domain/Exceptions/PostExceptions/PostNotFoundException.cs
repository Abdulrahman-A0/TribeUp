using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.PostExceptions
{
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException(int Id) : base($"Post With Id '{Id}' Was Not Found!")
        {
            
        }
    }
}
