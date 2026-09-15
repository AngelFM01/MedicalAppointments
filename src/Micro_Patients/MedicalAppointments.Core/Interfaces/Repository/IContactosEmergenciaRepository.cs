using MedicalAppointments.Domain.Model;
using Nuget_Persistence.Abstractions;

namespace MedicalAppointments.Core.Interfaces.Repository;

public interface IContactosEmergenciaRepository : IRepository<ContactoEmergencia>
{
    Task<ContactoEmergencia?> GetByIdAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactoEmergencia>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactoEmergencia>> GetByPacienteIdAsync(long pacienteId, CancellationToken cancellationToken = default);

    Task<ContactoEmergencia> AddAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default);

    Task UpdateAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default);
}
