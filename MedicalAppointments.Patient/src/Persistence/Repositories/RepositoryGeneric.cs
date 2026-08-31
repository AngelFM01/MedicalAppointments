using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class RepositoryGeneric<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public RepositoryGeneric(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
        {
            var entity = await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;    
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().AnyAsync(predicate, cancellationToken);
        }

        public async Task<string> GenerateNextCodigoAsync(CancellationToken cancellationToken)
        {
            // Obtener el último código numérico
            var last = await _context.Set<Paciente>()
                .OrderByDescending(p => p.CodigoPaciente)
                .Select(p => p.CodigoPaciente)
                .FirstOrDefaultAsync(cancellationToken);

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(last))
            {
                var parts = last.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                    nextNumber = num + 1;
            }

            return $"PAC-{nextNumber:D4}"; // PAC-0001, PAC-0002, ...
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }
        

        public async Task<IReadOnlyList<T>> GetByFilterAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>>? orderBy = null, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<T>().AsNoTracking().Where(filter);
            if (orderBy != null)
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var result = await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
            return result;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
