using Core.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories.Generic;

/// <summary>
/// Algoritmo que transforma una <see cref="ISpecification{TEntity}"/> declarativa en
/// un <see cref="IQueryable{TEntity}"/> de EF Core, aplicando en orden:
/// 1) seguimiento de cambios, 2) filtro, 3) includes, 4) ordenamiento, 5) paginación.
/// De esta forma la lógica de armado de consultas se escribe una sola vez.
/// </summary>
/// <typeparam name="TEntity">Entidad consultada.</typeparam>
internal static class SpecificationEvaluator<TEntity> where TEntity : class
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> specification)
    {
        var query = inputQuery;

        if (specification.AsNoTracking)
            query = query.AsNoTracking();

        if (specification.Criteria is not null)
            query = query.Where(specification.Criteria);

        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending is not null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.Skip is not null)
            query = query.Skip(specification.Skip.Value);

        if (specification.Take is not null)
            query = query.Take(specification.Take.Value);

        return query;
    }
}
