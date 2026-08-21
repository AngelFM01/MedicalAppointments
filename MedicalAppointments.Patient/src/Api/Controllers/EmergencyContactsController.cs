using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("patients/{pacienteId:long}/emergency-contacts")]
public sealed class ContactosEmergenciaController(IPatientsRepository patientsRepository, IContactosEmergenciaRepository contactsRepository) : ControllerBase
{
    [HttpGet("/EmergencyContacts")]
    public async Task<ActionResult<IReadOnlyList<ContactoEmergencia>>> GetAllContacts(CancellationToken cancellationToken) =>
        Ok(await contactsRepository.GetAllAsync(cancellationToken));

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContactoEmergencia>>> GetAll(long pacienteId, CancellationToken cancellationToken)
    {
        if (await patientsRepository.GetByIdAsync(pacienteId, cancellationToken) is null) return NotFound();
        return Ok(await contactsRepository.GetByPacienteIdAsync(pacienteId, cancellationToken));
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

    [HttpPut("{contactoEmergenciaId:long}")]
    public async Task<IActionResult> Update(long pacienteId, long contactoEmergenciaId, ContactoEmergencia contacto, CancellationToken cancellationToken)
    {
        var existing = await contactsRepository.GetByIdAsync(contactoEmergenciaId, cancellationToken);
        if (existing is null || existing.PacienteId != pacienteId) return NotFound();
        contacto.ContactoEmergenciaId = contactoEmergenciaId;
        contacto.PacienteId = pacienteId;
        await contactsRepository.UpdateAsync(contacto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{contactoEmergenciaId:long}")]
    public async Task<IActionResult> Delete(long pacienteId, long contactoEmergenciaId, CancellationToken cancellationToken)
    {
        var existing = await contactsRepository.GetByIdAsync(contactoEmergenciaId, cancellationToken);
        return existing is null || existing.PacienteId != pacienteId
            ? NotFound()
            : await contactsRepository.DeleteAsync(contactoEmergenciaId, cancellationToken) ? NoContent() : NotFound();
    }
}
