using Domain.Contracts;
using Domain.Entities;
using Persistence.Data.Contexts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;
        private readonly ConcurrentDictionary<string, object> repositories;
        public UnitOfWork(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            repositories = new();
        }
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            return (IGenericRepository<TEntity, TKey>)repositories.GetOrAdd(typeof(TEntity).Name, (_) => new GenericRepository<TEntity, TKey>(dbContext));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
