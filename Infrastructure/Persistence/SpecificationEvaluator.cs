using Domain.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(IQueryable<TEntity> inputQuery,
            ISpecifications<TEntity, TKey> specifications) where TEntity : BaseEntity<TKey>
        {
            if(specifications.Criteria is not null)
                inputQuery = inputQuery.Where(specifications.Criteria);


            if (specifications.IncludeExpressions is not null && specifications.IncludeExpressions.Count > 0)
                inputQuery = specifications.IncludeExpressions.Aggregate(inputQuery,
                    (currentQuery, expression) => currentQuery.Include(expression));


            if (specifications.ThenIncludeExpressions != null && specifications.ThenIncludeExpressions.Count > 0)
            {
                foreach (var thenInclude in specifications.ThenIncludeExpressions)
                {
                    inputQuery = thenInclude(inputQuery);
                }
            }

            return inputQuery;
        }
    }
}
