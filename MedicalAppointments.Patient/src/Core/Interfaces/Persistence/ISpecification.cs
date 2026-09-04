using System.Linq.Expressions;

namespace Core.Interfaces.Persistence;

/// <summary>
/// Describe de forma declarativa y reutilizable una consulta sobre <typeparamref name="TEntity"/>:
/// filtro, inclusiones de propiedades de navegación, ordenamiento y paginación.
/// El repositorio genérico traduce la especificación a una consulta ejecutable,
/// evitando repetir LINQ en cada caso de uso.
/// </summary>
/// <typeparam name="TEntity">Entidad de dominio consultada.</typeparam>
public interface ISpecification<TEntity> where TEntity : class
{
    /// <summary>Predicado de filtrado (cláusula WHERE). <c>null</c> = sin filtro.</summary>
    Expression<Func<TEntity, bool>>? Criteria { get; }

    /// <summary>Propiedades de navegación a incluir (eager loading).</summary>
    IReadOnlyList<Expression<Func<TEntity, object>>> Includes { get; }

    /// <summary>Ordenamiento ascendente. Mutuamente excluyente con <see cref="OrderByDescending"/>.</summary>
    Expression<Func<TEntity, object>>? OrderBy { get; }

    /// <summary>Ordenamiento descendente. Mutuamente excluyente con <see cref="OrderBy"/>.</summary>
    Expression<Func<TEntity, object>>? OrderByDescending { get; }

    /// <summary>Número de registros a omitir (paginación). <c>null</c> = sin paginación.</summary>
    int? Skip { get; }

    /// <summary>Número máximo de registros a devolver (paginación). <c>null</c> = sin límite.</summary>
    int? Take { get; }

    /// <summary>Cuando es <c>true</c> la consulta se ejecuta sin seguimiento de cambios (solo lectura).</summary>
    bool AsNoTracking { get; }
}
