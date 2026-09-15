using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Nuget_Persistence.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(object id, bool asNoTracking = false, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> GetTopAsync(int count, bool asNoTracking = true, CancellationToken cancellationToken = default);
        Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = true, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}

