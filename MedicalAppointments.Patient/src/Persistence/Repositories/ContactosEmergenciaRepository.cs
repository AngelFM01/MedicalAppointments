using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Repositories.Generic;

namespace Persistence.Repositories;

/// <summary>
/// Repositorio concreto de <see cref="ContactoEmergencia"/>. Reutiliza el CRUD del
/// <see cref="GenericRepository{TEntity, TKey}"/> y sólo redefine el ordenamiento por defecto,
/// agrega la consulta por paciente y envuelve las escrituras con <c>SaveChangesAsync</c> inmediato.
/// </summary>
public sealed class ContactosEmergenciaRepository(AppDbContext context)
    : GenericRepository<ContactoEmergencia, long>(context), IContactosEmergenciaRepository
{
    public override async Task<IReadOnlyList<ContactoEmergencia>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking()
            .OrderBy(x => x.PacienteId).ThenBy(x => x.Prioridad)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ContactoEmergencia>> GetByPacienteIdAsync(long pacienteId, CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking()
            .Where(x => x.PacienteId == pacienteId)
            .OrderBy(x => x.Prioridad)
            .ToListAsync(cancellationToken);

    public override async Task<ContactoEmergencia> AddAsync(ContactoEmergencia entity, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(ContactoEmergencia contacto, CancellationToken cancellationToken = default)
    {
        base.Update(contacto);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long contactoEmergenciaId, CancellationToken cancellationToken = default)
    {
        var removed = await RemoveByIdAsync(contactoEmergenciaId, cancellationToken);
        if (removed) await SaveChangesAsync(cancellationToken);
        return removed;
    }
}
