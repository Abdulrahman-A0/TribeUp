using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupExceptions
{
    public class GroupNotFoundException : NotFoundException
    {
        public GroupNotFoundException(int Id) : base($"Group With Id '{Id}' Was Not Found!")
        {

        }
    }
}
