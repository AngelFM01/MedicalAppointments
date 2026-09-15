using MedicalAppointments.Core.Feature.EmergencyContacts.Commands;
using MedicalAppointments.Core.Feature.EmergencyContacts.Queries;
using MedicalAppointments.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuget_Persistence.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("patients/{pacienteId:long}/EmergencyContacts")]
public sealed class ContactosEmergenciaController(IMediator mediator) : ControllerBase
{
    [HttpGet("/EmergencyContacts")]
    public async Task<ActionResult<IReadOnlyList<ContactoEmergencia>>> GetAllContacts(CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new GetAllEmergencyContactsQuery(), cancellationToken));

    [HttpGet("/EmergencyContacts/paged")]
    public async Task<ActionResult<PagedResult<ContactoEmergencia>>> GetPagedContacts(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? filter,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(new GetEmergencyContactsPagedQuery
            {
                PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                PageSize = pageSize <= 0 ? 20 : pageSize,
                Filter = filter
            }, cancellationToken);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContactoEmergencia>>> GetAll(long pacienteId, CancellationToken cancellationToken)
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

    [HttpPost]
    public async Task<ActionResult<ContactoEmergencia>> Create(long pacienteId, [FromBody] CreateEmergencyContactCommand command, CancellationToken cancellationToken)
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

    [HttpPut("{contactoEmergenciaId:long}")]
    public async Task<IActionResult> Update(long pacienteId, long contactoEmergenciaId, [FromBody] UpdateEmergencyContactCommand command, CancellationToken cancellationToken)
    {
        command.PacienteId = pacienteId;
        command.ContactoEmergenciaId = contactoEmergenciaId;
        var updated = await mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{contactoEmergenciaId:long}")]
    public async Task<IActionResult> Delete(long pacienteId, long contactoEmergenciaId, CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(new DeleteEmergencyContactCommand { PacienteId = pacienteId, ContactoEmergenciaId = contactoEmergenciaId }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
