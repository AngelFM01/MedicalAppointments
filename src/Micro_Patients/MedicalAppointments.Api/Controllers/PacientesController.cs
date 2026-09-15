using MedicalAppointments.Core.Feature.Patients.Commands;
using MedicalAppointments.Core.Feature.Patients.Queries;
using MedicalAppointments.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuget_Persistence.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("patients")]
public sealed class PacientesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Paciente>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPacientesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<Paciente>>> GetPaged(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? filter,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(new GetPacientesPagedQuery
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

    [HttpGet("{pacienteId:long}")]
    public async Task<ActionResult<Paciente>> GetById(long pacienteId, CancellationToken cancellationToken)
    {
        var paciente = await mediator.Send(new GetPacienteByIdQuery { PacienteId = pacienteId }, cancellationToken);
        return paciente is null ? NotFound() : Ok(paciente);
    }

    [HttpPost]
    public async Task<ActionResult<Paciente>> Create([FromBody] CreatePacienteCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var paciente = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { pacienteId = paciente.PacienteId }, paciente);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message, Status = StatusCodes.Status409Conflict });
        }
    }

    [HttpPut("{pacienteId:long}")]
    public async Task<IActionResult> Update(long pacienteId, [FromBody] UpdatePacienteCommand command, CancellationToken cancellationToken)
    {
        command.PacienteId = pacienteId;
        try
        {
            var updated = await mediator.Send(command, cancellationToken);
            return updated ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message, Status = StatusCodes.Status409Conflict });
        }
    }

    [HttpDelete("{pacienteId:long}")]
    public async Task<IActionResult> Delete(long pacienteId, CancellationToken cancellationToken) =>
        await mediator.Send(new DeletePacienteCommand { PacienteId = pacienteId }, cancellationToken) ? NoContent() : NotFound();
}
