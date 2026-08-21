using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories;

public sealed class ContactosEmergenciaRepository(AppDbContext context) : IContactosEmergenciaRepository
{
    public async Task<IReadOnlyList<ContactoEmergencia>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.ContactosEmergencia.AsNoTracking()
            .OrderBy(x => x.PacienteId).ThenBy(x => x.Prioridad)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ContactoEmergencia>> GetByPacienteIdAsync(long pacienteId, CancellationToken cancellationToken = default) =>
        await context.ContactosEmergencia.AsNoTracking().Where(x => x.PacienteId == pacienteId).OrderBy(x => x.Prioridad).ToListAsync(cancellationToken);

    public Task<ContactoEmergencia?> GetByIdAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default) =>
        context.ContactosEmergencia.AsNoTracking().FirstOrDefaultAsync(x => x.ContactoEmergenciaId == contactoEmergenciaId, cancellationToken);

    public async Task AddAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default)
    {
        await context.ContactosEmergencia.AddAsync(contacto, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default)
    {
        context.ContactosEmergencia.Update(contacto);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default)
    {
        var contacto = await context.ContactosEmergencia.FindAsync([contactoEmergenciaId], cancellationToken);
        if (contacto is null) return false;
        context.ContactosEmergencia.Remove(contacto);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
