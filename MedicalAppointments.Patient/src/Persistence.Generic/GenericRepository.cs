using System.Linq.Expressions;
using Core.Interfaces.Persistence;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Generic;

/// <summary>
/// Implementación única y reutilizable de <see cref="IGenericRepository{TEntity, TKey}"/> sobre EF Core.
/// Depende únicamente de <see cref="DbContext"/> (no de un <c>DbContext</c> concreto de la aplicación),
/// por lo que esta clase vive en una biblioteca de persistencia independiente y puede reutilizarse en
/// cualquier proyecto que tenga su propio <see cref="DbContext"/>. Resuelve el <see cref="DbSet{TEntity}"/>
/// dinámicamente con <c>context.Set&lt;TEntity&gt;()</c>, por lo que da servicio a cualquier entidad
/// <see cref="IEntity{TKey}"/> registrada en el modelo, sin escribir código adicional.
/// Los métodos son <c>virtual</c> para permitir que un repositorio concreto ajuste comportamientos puntuales
/// (por ejemplo el ordenamiento por defecto de <see cref="GetAllAsync"/>).
/// </summary>
/// <typeparam name="TEntity">Entidad de dominio gestionada.</typeparam>
/// <typeparam name="TKey">Tipo de la clave primaria.</typeparam>
public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    /// <summary>Contexto EF Core compartido (inyectado con ciclo de vida Scoped).</summary>
    protected DbContext Context { get; }

    /// <summary>Conjunto tipado de la entidad gestionada.</summary>
    protected DbSet<TEntity> Set => Context.Set<TEntity>();

    /// <summary>Nombre real de la columna/propiedad clave, resuelto por metadatos del modelo.</summary>
    private readonly string _keyName;

    public GenericRepository(DbContext context)
    {
        Context = context;
        _keyName = context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties[0].Name
                   ?? throw new InvalidOperationException(
                       $"La entidad '{typeof(TEntity).Name}' no tiene una clave primaria configurada en el modelo.");
    }

    // ---------------------- LECTURA ----------------------

    public virtual Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default) =>
        Set.AsNoTracking().FirstOrDefaultAsync(KeyEquals(id), cancellationToken);

    public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        Set.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator<TEntity>.GetQuery(Set, specification).ToListAsync(cancellationToken);

    public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        Set.AnyAsync(predicate, cancellationToken);

    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null
            ? Set.CountAsync(cancellationToken)
            : Set.CountAsync(predicate, cancellationToken);

    // ---------------------- ALTA ----------------------

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Set.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
        Set.AddRangeAsync(entities, cancellationToken);

    // ---------------------- MODIFICACIÓN ----------------------

    public virtual TEntity Update(TEntity entity)
    {
        Set.Update(entity);
        return entity;
    }

    // ---------------------- BAJA ----------------------

    public virtual void Remove(TEntity entity) => Set.Remove(entity);

    public virtual void RemoveRange(IEnumerable<TEntity> entities) => Set.RemoveRange(entities);

    public virtual async Task<bool> RemoveByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await Set.FirstOrDefaultAsync(KeyEquals(id), cancellationToken);
        if (entity is null) return false;
        Set.Remove(entity);
        return true;
    }

    // ---------------------- PERSISTENCIA ----------------------

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        Context.SaveChangesAsync(cancellationToken);

    // ---------------------- INFRAESTRUCTURA INTERNA ----------------------

    /// <summary>
    /// Construye dinámicamente el predicado <c>e =&gt; EF.Property&lt;TKey&gt;(e, keyName) == id</c>
    /// a partir del nombre de la clave obtenido de los metadatos, de modo que el repositorio
    /// no necesita conocer en compilación cómo se llama la propiedad clave de cada entidad.
    /// </summary>
    private Expression<Func<TEntity, bool>> KeyEquals(TKey id)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var keyAccess = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [typeof(TKey)],
            parameter,
            Expression.Constant(_keyName));
        var body = Expression.Equal(keyAccess, Expression.Constant(id, typeof(TKey)));
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}
