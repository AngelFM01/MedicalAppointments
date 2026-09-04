using System.Linq.Expressions;

namespace Core.Interfaces.Persistence;

/// <summary>
/// Implementación base reutilizable de <see cref="ISpecification{TEntity}"/>.
/// Las especificaciones concretas heredan de esta clase y componen la consulta
/// usando los métodos protegidos (<see cref="Where"/>, <see cref="AddInclude"/>,
/// <see cref="ApplyOrderBy"/>, <see cref="ApplyPaging"/>...).
/// </summary>
/// <typeparam name="TEntity">Entidad de dominio consultada.</typeparam>
public abstract class BaseSpecification<TEntity> : ISpecification<TEntity> where TEntity : class
{
    private readonly List<Expression<Func<TEntity, object>>> _includes = [];

    /// <summary>Crea una especificación sin filtro.</summary>
    protected BaseSpecification() { }

    /// <summary>Crea una especificación con el filtro indicado.</summary>
    protected BaseSpecification(Expression<Func<TEntity, bool>> criteria) => Criteria = criteria;

    public Expression<Func<TEntity, bool>>? Criteria { get; private set; }
    public IReadOnlyList<Expression<Func<TEntity, object>>> Includes => _includes;
    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }
    public int? Skip { get; private set; }
    public int? Take { get; private set; }
    public bool AsNoTracking { get; private set; } = true;

    /// <summary>Define o reemplaza el predicado de filtrado.</summary>
    protected void Where(Expression<Func<TEntity, bool>> criteria) => Criteria = criteria;

    /// <summary>Agrega una propiedad de navegación al eager loading.</summary>
    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression) => _includes.Add(includeExpression);

    /// <summary>Aplica ordenamiento ascendente.</summary>
    protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExpression) => OrderBy = orderByExpression;

    /// <summary>Aplica ordenamiento descendente.</summary>
    protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression) => OrderByDescending = orderByDescExpression;

    /// <summary>Aplica paginación (omitir <paramref name="skip"/> y tomar <paramref name="take"/>).</summary>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    /// <summary>Habilita el seguimiento de cambios para esta consulta (por defecto está deshabilitado).</summary>
    protected void EnableTracking() => AsNoTracking = false;
}
