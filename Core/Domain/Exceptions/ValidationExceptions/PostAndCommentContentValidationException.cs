using Domain.Exceptions.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.ValidationExceptions
{
    public sealed class PostAndCommentContentValidationException : ValidationException
    {
        public PostAndCommentContentValidationException(
            IDictionary<string, string[]> errors)
            : base(errors)
        {
            
        }
    }
}
