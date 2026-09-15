using MedicalAppointments.Core.Interfaces.Repository;
using MedicalAppointments.Domain.Model;
using MedicalAppointments.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Nuget_Persistence.Implementations;

namespace MedicalAppointments.Persistence.Repositories;

public sealed class PatientsRepository : Repository<Paciente>, IPatientsRepository
{
    private readonly AppDbContext _context;

    public PatientsRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<Paciente?> GetByIdAsync(long pacienteId, CancellationToken cancellationToken = default) =>
        _context.Pacientes.AsNoTracking().FirstOrDefaultAsync(x => x.PacienteId == pacienteId, cancellationToken);

    public async Task<IReadOnlyList<Paciente>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Pacientes.AsNoTracking().OrderBy(x => x.Apellidos).ThenBy(x => x.Nombres).ToListAsync(cancellationToken);

    public Task<bool> ExistsByDocumentAsync(string tipoDocumento, string numeroDocumento, long? excludingPacienteId = null, CancellationToken cancellationToken = default) =>
        _context.Pacientes.AnyAsync(
            x => x.TipoDocumento == tipoDocumento
                 && x.NumeroDocumento == numeroDocumento
                 && (!excludingPacienteId.HasValue || x.PacienteId != excludingPacienteId),
            cancellationToken);

    public new async Task<Paciente> AddAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(paciente, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return paciente;
    }

    public async Task UpdateAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        base.Update(paciente);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long pacienteId, CancellationToken cancellationToken = default)
    {
        var paciente = await _context.Pacientes.FirstOrDefaultAsync(x => x.PacienteId == pacienteId, cancellationToken);
        if (paciente is null) return false;

        base.Remove(paciente);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
