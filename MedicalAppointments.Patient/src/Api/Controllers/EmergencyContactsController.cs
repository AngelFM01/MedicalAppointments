using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("patients/{pacienteId:long}/emergency-contacts")]
public sealed class ContactosEmergenciaController(IRepository<Paciente> patientsRepository, IRepository<ContactoEmergencia> contactsRepository) : ControllerBase
{
    [HttpGet("/EmergencyContacts")]
    public async Task<ActionResult<IReadOnlyList<ContactoEmergencia>>> GetAllContacts(CancellationToken cancellationToken) =>
        Ok(await contactsRepository.GetAllAsync(cancellationToken));

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContactoEmergencia>>> GetAll(long pacienteId, CancellationToken cancellationToken)
    {
        if (await patientsRepository.GetByIdAsync(pacienteId, cancellationToken) is null) return NotFound();
        return Ok(await contactsRepository.GetByFilterAsync(c => c.PacienteId == pacienteId, cancellationToken: cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ContactoEmergencia>> Create(long pacienteId, ContactoEmergencia contacto, CancellationToken cancellationToken)
    {
        if (await patientsRepository.GetByIdAsync(pacienteId, cancellationToken) is null) return NotFound();
        contacto.ContactoEmergenciaId = 0;
        contacto.PacienteId = pacienteId;
        await contactsRepository.AddAsync(contacto, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { pacienteId }, contacto);
    }

    [HttpPut("{contactoId:long}")]
    public async Task<IActionResult> Update(long contactoId, ContactoEmergencia contacto, CancellationToken cancellationToken)
    {
        // 1. Obtener la entidad existente (ya rastreada por el contexto)
        var existing = await contactsRepository.GetByIdAsync(contactoId, cancellationToken);
        if (existing is null)
            return NotFound();

        // 2. (Opcional) Validar que el paciente exista si estás cambiando el PacienteId
        if (contacto.PacienteId != existing.PacienteId)
        {
            var pacienteExiste = await patientsRepository.ExistsAsync(p => p.PacienteId == contacto.PacienteId, cancellationToken);
            if (!pacienteExiste)
                return Conflict(new ProblemDetails { Detail = "El paciente especificado no existe.", Status = StatusCodes.Status409Conflict });
        }

        // 3. Mapear los valores al objeto rastreado
        existing.PacienteId = contacto.PacienteId;
        existing.NombreCompleto = contacto.NombreCompleto;
        existing.Parentesco = contacto.Parentesco;
        existing.Telefono = contacto.Telefono;
        existing.TelefonoSecundario = contacto.TelefonoSecundario;
        existing.Email = contacto.Email;
        existing.Prioridad = contacto.Prioridad;
        existing.Activo = contacto.Activo;
        // No modifiques ContactoEmergenciaId

        // 4. Guardar cambios (el contexto ya rastrea `existing`)
        await contactsRepository.UpdateAsync(existing, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{contactoEmergenciaId:long}")]
    public async Task<IActionResult> Delete(long pacienteId, long contactoEmergenciaId, CancellationToken cancellationToken)
    {
        var existing = await contactsRepository.GetByIdAsync(contactoEmergenciaId, cancellationToken);
        if (existing is null)
            return NotFound();

        // Soft delete
        existing.Activo = false;
        await contactsRepository.UpdateAsync(existing, cancellationToken);

        return NoContent();
    }
}
