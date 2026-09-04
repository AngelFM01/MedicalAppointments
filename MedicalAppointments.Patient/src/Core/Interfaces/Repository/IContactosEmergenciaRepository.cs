using Core.Interfaces.Persistence;
using Domain.Model;

namespace Core.Interfaces.Repository;

/// <summary>
/// Repositorio de <see cref="ContactoEmergencia"/>. Hereda el CRUD genérico de
/// <see cref="IGenericRepository{TEntity, TKey}"/> y añade sólo lo específico del contacto.
/// </summary>
public interface IContactosEmergenciaRepository : IGenericRepository<ContactoEmergencia, long>
{
    /// <summary>Devuelve los contactos de un paciente, ordenados por prioridad.</summary>
    Task<IReadOnlyList<ContactoEmergencia>> GetByPacienteIdAsync(long pacienteId, CancellationToken cancellationToken = default);

    /// <summary>Modificación con guardado inmediato (compatibilidad con los handlers actuales).</summary>
    Task UpdateAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default);

    /// <summary>Baja por Id con guardado inmediato. Devuelve <c>false</c> si el contacto no existe.</summary>
    Task<bool> DeleteAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default);
}
