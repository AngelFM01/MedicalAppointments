using Core.Interfaces.Persistence;
using Domain.Model;

namespace Core.Interfaces.Repository;

/// <summary>
/// Repositorio de <see cref="Paciente"/>. Hereda todas las operaciones CRUD genéricas
/// de <see cref="IGenericRepository{TEntity, TKey}"/> y añade únicamente lo específico del paciente.
/// </summary>
public interface IPatientsRepository : IGenericRepository<Paciente, long>
{
    /// <summary>Indica si ya existe un paciente con ese tipo y número de documento (opcionalmente excluyendo un Id).</summary>
    Task<bool> ExistsByDocumentAsync(string tipoDocumento, string numeroDocumento, long? excludingPacienteId = null, CancellationToken cancellationToken = default);

    /// <summary>Alta con guardado inmediato (compatibilidad con los handlers actuales).</summary>
    Task UpdateAsync(Paciente paciente, CancellationToken cancellationToken = default);

    /// <summary>Baja por Id con guardado inmediato. Devuelve <c>false</c> si el paciente no existe.</summary>
    Task<bool> DeleteAsync(long pacienteId, CancellationToken cancellationToken = default);
}
