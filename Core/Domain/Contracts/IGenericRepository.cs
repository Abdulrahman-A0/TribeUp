using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false);
        Task<TEntity?> GetByIdAsync(TKey Id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);


        #region Specifications
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications);
        Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications);
        #endregion
    }
}
