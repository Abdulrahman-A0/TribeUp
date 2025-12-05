using Domain.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    public class BaseSpecifications<TEntity, TKey> : ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        #region Where
        protected BaseSpecifications(Expression<Func<TEntity, bool>>? criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<TEntity, bool>>? Criteria { get; private set; }
        #endregion


        #region Include
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = new();
        protected void AddIncludes(Expression<Func<TEntity, object>> includeExpressions)
        {
            IncludeExpressions.Add(includeExpressions);
        }
        #endregion

    }
}
