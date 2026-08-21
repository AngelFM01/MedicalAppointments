using Domain.Model;

namespace Core.Interfaces.Repository;

public interface IContactosEmergenciaRepository
{
    Task<IReadOnlyList<ContactoEmergencia>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactoEmergencia>> GetByPacienteIdAsync(long pacienteId, CancellationToken cancellationToken = default);
    Task<ContactoEmergencia?> GetByIdAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default);
    Task AddAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default);
    Task UpdateAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default);
}
