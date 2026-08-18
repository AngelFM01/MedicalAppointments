using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories;

public class PatientRepository(AppDbContext context) : IPatients
{
    public Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default) =>
        context.Patients.AnyAsync(patient => patient.DocumentNumber == documentNumber, cancellationToken);

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await context.Patients.AddAsync(patient, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
