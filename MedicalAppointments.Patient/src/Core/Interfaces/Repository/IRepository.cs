using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository
{
    public interface IRepository<T> where T : class
    {
        //Consultas
        Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<T>> GetByFilterAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>>? orderBy = null,
        bool ascending = true, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        //Comandos
        Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
        Task AddAsync(T entity, CancellationToken cancellationToken);
        Task UpdateAsync(T entity, CancellationToken cancellationToken);
        Task<string> GenerateNextCodigoAsync(CancellationToken cancellationToken);

    }
}
