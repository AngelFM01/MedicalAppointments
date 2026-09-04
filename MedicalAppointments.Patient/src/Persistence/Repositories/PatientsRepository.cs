using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Repositories.Generic;

namespace Persistence.Repositories;

/// <summary>
/// Repositorio concreto de <see cref="Paciente"/>. Reutiliza todo el CRUD del
/// <see cref="GenericRepository{TEntity, TKey}"/> y sólo:
///  - redefine <see cref="GetAllAsync"/> para conservar el ordenamiento por apellidos/nombres;
///  - agrega la validación de documento única del paciente;
///  - envuelve las escrituras con <c>SaveChangesAsync</c> inmediato (compatibilidad con los handlers actuales).
/// El resto de operaciones (GetById, List, Exists, Count, etc.) se heredan sin escribir código.
/// </summary>
public sealed class PatientsRepository(AppDbContext context)
    : GenericRepository<Paciente, long>(context), IPatientsRepository
{
    public override async Task<IReadOnlyList<Paciente>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking().OrderBy(x => x.Apellidos).ThenBy(x => x.Nombres).ToListAsync(cancellationToken);

    public Task<bool> ExistsByDocumentAsync(string tipoDocumento, string numeroDocumento, long? excludingPacienteId = null, CancellationToken cancellationToken = default) =>
        ExistsAsync(
            x => x.TipoDocumento == tipoDocumento
                 && x.NumeroDocumento == numeroDocumento
                 && (!excludingPacienteId.HasValue || x.PacienteId != excludingPacienteId),
            cancellationToken);

    public override async Task<Paciente> AddAsync(Paciente entity, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        base.Update(paciente);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(long pacienteId, CancellationToken cancellationToken = default)
    {
        var removed = await RemoveByIdAsync(pacienteId, cancellationToken);
        if (removed) await SaveChangesAsync(cancellationToken);
        return removed;
    }
}
