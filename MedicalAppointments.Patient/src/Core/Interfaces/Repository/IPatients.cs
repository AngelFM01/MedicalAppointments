using Domain.Model;

namespace Core.Interfaces.Repository;

public interface IPatients
{
    Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
}
