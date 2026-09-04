namespace Domain.Abstractions;

/// <summary>
/// Contrato genérico que identifica a cualquier entidad de dominio persistible
/// mediante su clave primaria. Se usa como restricción (<c>where TEntity : IEntity&lt;TKey&gt;</c>)
/// en el repositorio genérico para poder trabajar con "objetos genéricos"
/// sin acoplarse a un tipo concreto.
/// </summary>
/// <typeparam name="TKey">Tipo de la clave primaria (por ejemplo <see cref="long"/>, <see cref="int"/> o <see cref="System.Guid"/>).</typeparam>
public interface IEntity<out TKey> where TKey : notnull
{
    /// <summary>Valor de la clave primaria de la entidad.</summary>
    TKey Id { get; }
}
