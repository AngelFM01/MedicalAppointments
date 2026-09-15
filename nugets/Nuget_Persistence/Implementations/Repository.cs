using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nuget_Persistence.Abstractions;
using Nuget_Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Nuget_Persistence.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
       private readonly DbContext _context;
       private readonly DbSet<TEntity> _dbSet;
        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(object id, bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(id);

            if (!asNoTracking)
                return await _dbSet.FindAsync(new[] { id }, cancellationToken);

            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            var property = Expression.Property(parameter, GetPrimaryKeyProperty().Name);
            var value = Expression.Constant(id, property.Type);
            var predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(property, value), parameter);

            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await Query(asNoTracking).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<TEntity>> GetTopAsync(int count, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "El número de registros a traer debe ser mayor a cero.");

            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            var property = Expression.Property(parameter, GetPrimaryKeyProperty().Name);
            var keySelector = Expression.Lambda<Func<TEntity, object>>(Expression.Convert(property, typeof(object)), parameter);

            return await Query(asNoTracking).OrderBy(keySelector).Take(count).ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return await Query(asNoTracking).FirstOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<PagedResult<TEntity>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool asNoTracking = true,
            bool splitQuery = false,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "pageNumber debe ser 1 o mayor.");
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "pageSize debe ser 1 o mayor.");

            var query = ApplyIncludes(Query(asNoTracking), includes);
            if (filter != null) query = query.Where(filter);
            if (splitQuery) query = query.AsSplitQuery();

            // Cuenta sobre la consulta ya filtrada (pero todavía sin ordenar/paginar).
            var totalRecords = await query.CountAsync(cancellationToken);

            query = orderBy != null ? orderBy(query) : query;
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var data = await query.ToListAsync(cancellationToken);

            return new PagedResult<TEntity>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageSize = pageSize,
                CurrentPage = pageNumber
            };
        }

        private IQueryable<TEntity> Query(bool asNoTracking)
        {
            IQueryable<TEntity> query = _dbSet;
            return asNoTracking ? query.AsNoTracking() : query;
        }

        private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, Expression<Func<TEntity, object>>[] includes)
        {
            foreach (var include in includes)
                query = query.Include(include);
            return query;
        }

        private IProperty GetPrimaryKeyProperty() =>
            _context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.FirstOrDefault()
                ?? throw new InvalidOperationException($"No se pudo determinar la clave primaria de '{typeof(TEntity).Name}'.");

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await _dbSet.AddAsync(entity, cancellationToken);
        }
                    
        public async void Remove(TEntity entity)
        {
            
            ArgumentNullException.ThrowIfNull(entity);
            await Task.Run(() =>
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                _context.Remove(entity);
            });
            
        }

        public async void Update(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
