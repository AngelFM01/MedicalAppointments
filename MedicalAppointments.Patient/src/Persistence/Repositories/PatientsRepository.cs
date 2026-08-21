using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories;

public sealed class PatientsRepository(AppDbContext context) : IPatientsRepository
{
    public async Task<IReadOnlyList<Paciente>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Pacientes.AsNoTracking().OrderBy(x => x.Apellidos).ThenBy(x => x.Nombres).ToListAsync(cancellationToken);

    public Task<Paciente?> GetByIdAsync(long pacienteId, CancellationToken cancellationToken = default) =>
        context.Pacientes.AsNoTracking().FirstOrDefaultAsync(x => x.PacienteId == pacienteId, cancellationToken);

    public Task<bool> ExistsByDocumentAsync(string tipoDocumento, string numeroDocumento, long? excludingPacienteId = null, CancellationToken cancellationToken = default) =>
        context.Pacientes.AnyAsync(x => x.TipoDocumento == tipoDocumento && x.NumeroDocumento == numeroDocumento && (!excludingPacienteId.HasValue || x.PacienteId != excludingPacienteId), cancellationToken);

    public async Task AddAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        await context.Pacientes.AddAsync(paciente, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        context.Pacientes.Update(paciente);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long pacienteId, CancellationToken cancellationToken = default)
    {
        var paciente = await context.Pacientes.FindAsync([pacienteId], cancellationToken);
        if (paciente is null) return false;
        context.Pacientes.Remove(paciente);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
