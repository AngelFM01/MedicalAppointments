using System.Linq.Expressions;
using Domain.Abstractions;

namespace Core.Interfaces.Persistence;

/// <summary>
/// Contrato genérico de persistencia (patrón Repository) que expone todas las
/// operaciones CRUD sobre cualquier entidad <typeparamref name="TEntity"/> que
/// implemente <see cref="IEntity{TKey}"/>. Una única implementación concreta
/// (<c>GenericRepository&lt;TEntity, TKey&gt;</c>) da servicio a todas las entidades.
/// </summary>
/// <typeparam name="TEntity">Entidad de dominio. Debe ser una clase de referencia e <see cref="IEntity{TKey}"/>.</typeparam>
/// <typeparam name="TKey">Tipo de la clave primaria de la entidad.</typeparam>
public interface IGenericRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    // ---------------------- LECTURA (Read) ----------------------

    /// <summary>Obtiene una entidad por su clave primaria (sin seguimiento de cambios). <c>null</c> si no existe.</summary>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>Devuelve la primera entidad que cumple el predicado, o <c>null</c>.</summary>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Devuelve todas las entidades del conjunto.</summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Devuelve todas las entidades que cumplen el predicado.</summary>
    Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Devuelve las entidades que resultan de aplicar una <see cref="ISpecification{TEntity}"/>.</summary>
    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>Indica si existe al menos una entidad que cumple el predicado.</summary>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Cuenta las entidades (todas, o las que cumplen el predicado si se indica).</summary>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    // ---------------------- ALTA (Create) ----------------------

    /// <summary>Marca una entidad para inserción. No persiste hasta llamar a <see cref="SaveChangesAsync"/>.</summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Marca un conjunto de entidades para inserción.</summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    // ---------------------- MODIFICACIÓN (Update) ----------------------

    /// <summary>Marca una entidad como modificada. No persiste hasta llamar a <see cref="SaveChangesAsync"/>.</summary>
    TEntity Update(TEntity entity);

    // ---------------------- BAJA (Delete) ----------------------

    /// <summary>Marca una entidad para eliminación.</summary>
    void Remove(TEntity entity);

    /// <summary>Marca un conjunto de entidades para eliminación.</summary>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>Busca por clave y marca para eliminación. Devuelve <c>false</c> si la entidad no existe.</summary>
    Task<bool> RemoveByIdAsync(TKey id, CancellationToken cancellationToken = default);

    // ---------------------- PERSISTENCIA ----------------------

    /// <summary>Confirma en la base de datos todos los cambios pendientes. Devuelve el número de filas afectadas.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
