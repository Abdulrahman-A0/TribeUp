using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; }
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; }
        public List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> ThenIncludeExpressions { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPaginated { get; }

    }
}
