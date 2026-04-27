using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.Posts;
using Microsoft.EntityFrameworkCore;
using Service.Specifications.PostSpecifications;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Persistence
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(IQueryable<TEntity> inputQuery,
            ISpecifications<TEntity, TKey> specifications, bool isCountQuery = false) where TEntity : BaseEntity<TKey>
        {
            if (specifications.Criteria is not null)
                inputQuery = inputQuery.Where(specifications.Criteria);


            if (isCountQuery) return inputQuery;


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


            if (specifications.OrderByExpressions != null && specifications.OrderByExpressions.Count > 0)
            {
                IOrderedQueryable<TEntity> orderedQuery = null!;

                for (int i = 0; i < specifications.OrderByExpressions.Count; i++)
                {
                    var sort = specifications.OrderByExpressions[i];

                    if (i == 0)
                    {
                        orderedQuery = sort.IsDescending
                            ? inputQuery.OrderByDescending(sort.Expression)
                            : inputQuery.OrderBy(sort.Expression);
                    }
                    else
                    {
                        orderedQuery = sort.IsDescending
                            ? orderedQuery.ThenByDescending(sort.Expression)
                            : orderedQuery.ThenBy(sort.Expression);
                    }
                }
                inputQuery = orderedQuery ?? inputQuery;
            }
            // To keep older versions of "OrderBy" and "OrderByDescending" properties working
            else if (specifications.OrderByDescending is not null)
            {
                inputQuery = inputQuery.OrderByDescending(specifications.OrderByDescending);
            }
            else if (specifications.OrderBy is not null)
            {
                inputQuery = inputQuery.OrderBy(specifications.OrderBy);
            }


            if (specifications.IsPaginated)
            {
                inputQuery = inputQuery.Skip(specifications.Skip).Take(specifications.Take);
            }

            return inputQuery;
        }
    }
}
