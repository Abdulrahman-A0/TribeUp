using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.PollExceptions
{
    public class PollNotFoundException : NotFoundException
    {
        public PollNotFoundException(int Id) : base("Poll Was Not Found")
        {
        }
    }
}
