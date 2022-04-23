using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
//using Play.CataLog.Service.Entities;

namespace Play.Common
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T entity);
        Task<IReadOnlyCollection<T>> GetAllAsync();
        //Add linq Expression for filter to know which entity is of interest
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T> GetAsync(Guid id);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        Task RemoveAsync(Guid id);
        Task UpdateDb(T entity);
    }
}