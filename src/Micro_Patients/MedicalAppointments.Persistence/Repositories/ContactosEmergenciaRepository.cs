using MedicalAppointments.Core.Interfaces.Repository;
using MedicalAppointments.Domain.Model;
using MedicalAppointments.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Nuget_Persistence.Implementations;

namespace MedicalAppointments.Persistence.Repositories;

public sealed class ContactosEmergenciaRepository : Repository<ContactoEmergencia>, IContactosEmergenciaRepository
{
    private readonly AppDbContext _context;

    public ContactosEmergenciaRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<ContactoEmergencia?> GetByIdAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default) =>
        _context.ContactosEmergencia.AsNoTracking().FirstOrDefaultAsync(x => x.ContactoEmergenciaId == contactoEmergenciaId, cancellationToken);

    public async Task<IReadOnlyList<ContactoEmergencia>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.ContactosEmergencia.AsNoTracking()
            .OrderBy(x => x.PacienteId).ThenBy(x => x.Prioridad)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ContactoEmergencia>> GetByPacienteIdAsync(long pacienteId, CancellationToken cancellationToken = default) =>
        await _context.ContactosEmergencia.AsNoTracking()
            .Where(x => x.PacienteId == pacienteId)
            .OrderBy(x => x.Prioridad)
            .ToListAsync(cancellationToken);

    public new async Task<ContactoEmergencia> AddAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(contacto, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return contacto;
    }

    public async Task UpdateAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default)
    {
        base.Update(contacto);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default)
    {
        var contacto = await _context.ContactosEmergencia.FirstOrDefaultAsync(x => x.ContactoEmergenciaId == contactoEmergenciaId, cancellationToken);
        if (contacto is null) return false;

        base.Remove(contacto);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
