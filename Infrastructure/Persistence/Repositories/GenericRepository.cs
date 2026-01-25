using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.Posts;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;
using Service.Specifications.PostSpecifications;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey>(AppDbContext dbContext) :
        IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false)
        {
            return asNoTracking ? await dbContext.Set<TEntity>().AsNoTracking().ToListAsync()
                : await dbContext.Set<TEntity>().AsTracking().ToListAsync();
        }
        public async Task<IEnumerable<Post>> GetAllAsync(ISpecifications<Post, int> spec)
        {
            var query = SpecificationEvaluator
                .CreateQuery(dbContext.Set<Post>(), spec);

            if (spec is PostFeedSpecification postSpec)
            {
                query = query.Where(p =>
                    p.UserId == postSpec.CurrentUserId
                    ||
                    !dbContext.Set<AIModeration>().Any(m =>
                        m.EntityType == ModeratedEntityType.Post &&
                        m.EntityId == p.Id &&
                        m.Status == ContentStatus.Denied
                    )
                );
            }

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey Id)
        {
            return await dbContext.Set<TEntity>().FindAsync(Id);
        }
        public async Task AddAsync(TEntity entity)
        {
            await dbContext.Set<TEntity>().AddAsync(entity);
        }
        public void Update(TEntity entity)
        {
            dbContext.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbContext.Set<TEntity>().CountAsync(predicate);
        }


        #region Specifications
        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await SpecificationEvaluator.CreateQuery(dbContext.Set<TEntity>(), specifications).ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await SpecificationEvaluator.CreateQuery(dbContext.Set<TEntity>(), specifications).FirstOrDefaultAsync();
        }
        #endregion
    }
}
