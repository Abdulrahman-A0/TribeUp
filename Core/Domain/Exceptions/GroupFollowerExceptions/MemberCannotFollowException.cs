using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.GroupFollowerExceptions
{
    public class MemberCannotFollowException : ConflictException
    {
        public MemberCannotFollowException() : base("Members are already following the group; you are already part of it")
        {
        }
    }
}
