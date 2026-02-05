using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public sealed class CannotDemoteCreatorException : ConflictException
    {
        public CannotDemoteCreatorException()
            : base("The group creator cannot be demoted from the admin role.")
        {
        }
    }
}
