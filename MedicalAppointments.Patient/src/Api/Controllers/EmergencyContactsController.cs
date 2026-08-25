using Core.Feature.EmergencyContacts.Commands;
using Core.Feature.EmergencyContacts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("patients/{pacienteId:long}/EmergencyContacts")]
public sealed class ContactosEmergenciaController(IMediator mediator) : ControllerBase
{
    // GET /EmergencyContacts -> lista global (ruta absoluta como en el original adaptado a CQRS)
    [HttpGet("/EmergencyContacts")]
    public async Task<ActionResult<IReadOnlyList<Domain.Model.ContactoEmergencia>>> GetAllContacts(CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new GetAllEmergencyContactsQuery(), cancellationToken));

    // GET patients/{pacienteId}/EmergencyContacts
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Domain.Model.ContactoEmergencia>>> GetAll(long pacienteId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(new GetEmergencyContactsByPacienteQuery { PacienteId = pacienteId }, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST patients/{pacienteId}/EmergencyContacts
    [HttpPost]
    public async Task<ActionResult<Domain.Model.ContactoEmergencia>> Create(long pacienteId, [FromBody] CreateEmergencyContactCommand command, CancellationToken cancellationToken)
    {
        command.PacienteId = pacienteId;
        try
        {
            var contacto = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { pacienteId }, contacto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // PUT patients/{pacienteId}/EmergencyContacts/{contactoEmergenciaId}
    [HttpPut("{contactoEmergenciaId:long}")]
    public async Task<IActionResult> Update(long pacienteId, long contactoEmergenciaId, [FromBody] UpdateEmergencyContactCommand command, CancellationToken cancellationToken)
    {
        command.PacienteId = pacienteId;
        command.ContactoEmergenciaId = contactoEmergenciaId;
        var updated = await mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    // DELETE patients/{pacienteId}/EmergencyContacts/{contactoEmergenciaId}
    [HttpDelete("{contactoEmergenciaId:long}")]
    public async Task<IActionResult> Delete(long pacienteId, long contactoEmergenciaId, CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(new DeleteEmergencyContactCommand { PacienteId = pacienteId, ContactoEmergenciaId = contactoEmergenciaId }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
