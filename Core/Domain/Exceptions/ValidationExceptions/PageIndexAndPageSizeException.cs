using Domain.Exceptions.Abstraction;
using System.Collections.Generic;

namespace Domain.Exceptions
{
    public sealed class PageIndexAndPageSizeException : ValidationException
    {
        public PageIndexAndPageSizeException(int page, int pageSize)
            : base(BuildErrors(page, pageSize))
        {
        }

        private static IDictionary<string, string[]> BuildErrors(int page, int pageSize)
        {
            var errors = new Dictionary<string, string[]>();

            if (page < 1)
            {
                errors.Add("page", new[]
                {
                    "Page index must be greater than or equal to 1."
                });
            }

            if (pageSize < 1)
            {
                errors.Add("pageSize", new[]
                {
                    "Page size must be greater than 0."
                });
            }

            return errors;
        }
    }
}
