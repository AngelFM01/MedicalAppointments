using MedicalAppointments.Domain.Model;
using Nuget_Persistence.Abstractions;

namespace MedicalAppointments.Core.Interfaces.Repository;

public interface IPatientsRepository : IRepository<Paciente>
{
    Task<Paciente?> GetByIdAsync(long pacienteId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Paciente>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsByDocumentAsync(string tipoDocumento, string numeroDocumento, long? excludingPacienteId = null, CancellationToken cancellationToken = default);

    Task<Paciente> AddAsync(Paciente paciente, CancellationToken cancellationToken = default);

    Task UpdateAsync(Paciente paciente, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long pacienteId, CancellationToken cancellationToken = default);
}
