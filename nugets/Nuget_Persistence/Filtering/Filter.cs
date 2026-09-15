using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Nuget_Persistence.Filtering
{
    public static class Filter
    {
        public static Expression<Func<TModel, bool>> FromStringExpression<TModel>(string query, string parameter = "x")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            try
            {
                var parameterExpression = Expression.Parameter(typeof(TModel), parameter);
                return (Expression<Func<TModel, bool>>)DynamicExpressionParser.ParseLambda(
                    new[] { parameterExpression }, null, query);
            }
            catch (Exception)
            {
                throw new ValidationException($"El filtro '{query}' no es una expresión válida.");
            }
        }
    }
}
