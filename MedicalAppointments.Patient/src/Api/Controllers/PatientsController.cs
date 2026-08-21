using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("patients")]
public sealed class PacientesController(IPatientsRepository patientsRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Paciente>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await patientsRepository.GetAllAsync(cancellationToken));

    [HttpGet("{pacienteId:long}")]
    public async Task<ActionResult<Paciente>> GetById(long pacienteId, CancellationToken cancellationToken)
    {
        var paciente = await patientsRepository.GetByIdAsync(pacienteId, cancellationToken);
        return paciente is null ? NotFound() : Ok(paciente);
    }

    [HttpPost]
    public async Task<ActionResult<Paciente>> Create(Paciente paciente, CancellationToken cancellationToken)
    {
        if (await patientsRepository.ExistsByDocumentAsync(paciente.TipoDocumento, paciente.NumeroDocumento, cancellationToken: cancellationToken))
            return Conflict(new ProblemDetails { Detail = "Ya existe un paciente con este tipo y número de documento.", Status = StatusCodes.Status409Conflict });

        paciente.PacienteId = 0;
        paciente.FechaRegistro = default;
        await patientsRepository.AddAsync(paciente, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { pacienteId = paciente.PacienteId }, paciente);
    }

    [HttpPut("{pacienteId:long}")]
    public async Task<IActionResult> Update(long pacienteId, Paciente paciente, CancellationToken cancellationToken)
    {
        var existing = await patientsRepository.GetByIdAsync(pacienteId, cancellationToken);
        if (existing is null) return NotFound();
        if (await patientsRepository.ExistsByDocumentAsync(paciente.TipoDocumento, paciente.NumeroDocumento, pacienteId, cancellationToken))
            return Conflict(new ProblemDetails { Detail = "Ya existe un paciente con este tipo y número de documento.", Status = StatusCodes.Status409Conflict });

        paciente.PacienteId = pacienteId;
        paciente.FechaRegistro = existing.FechaRegistro;
        await patientsRepository.UpdateAsync(paciente, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{pacienteId:long}")]
    public async Task<IActionResult> Delete(long pacienteId, CancellationToken cancellationToken) =>
        await patientsRepository.DeleteAsync(pacienteId, cancellationToken) ? NoContent() : NotFound();
}
