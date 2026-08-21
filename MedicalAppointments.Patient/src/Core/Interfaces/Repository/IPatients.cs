using Domain.Model;

namespace Core.Interfaces.Repository;

public interface IPatientsRepository
{
    Task<IReadOnlyList<Paciente>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Paciente?> GetByIdAsync(long pacienteId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDocumentAsync(string tipoDocumento, string numeroDocumento, long? excludingPacienteId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Paciente paciente, CancellationToken cancellationToken = default);
    Task UpdateAsync(Paciente paciente, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long pacienteId, CancellationToken cancellationToken = default);
}
