using Azure.Core;
using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Api.Controllers;

[ApiController]
[Route("c")]
public sealed class PacientesController(IRepository<Paciente> patientsRepository) : ControllerBase
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

    [HttpPost("Create")]
    public async Task<ActionResult<Paciente>> Create(Paciente paciente, CancellationToken cancellationToken)
    {
        //Validacion del documento del paciente para evitar duplicados
        if (await patientsRepository.ExistsAsync(p => p.TipoDocumento == paciente.TipoDocumento && p.NumeroDocumento == paciente.NumeroDocumento, cancellationToken: cancellationToken))
            return Conflict(new ProblemDetails { Detail = "Ya existe un paciente con este tipo y número de documento.", Status = StatusCodes.Status409Conflict });

        // Validación y generación automática de código
        if (string.IsNullOrWhiteSpace(paciente.CodigoPaciente))
        {
            // Si no viene código, lo generamos desde el repositorio
            paciente.CodigoPaciente = await patientsRepository.GenerateNextCodigoAsync(cancellationToken);
        }
        else
        {
            // Si viene código, validamos que no exista
            if (await patientsRepository.ExistsAsync(p => p.CodigoPaciente == paciente.CodigoPaciente, cancellationToken))
                return Conflict(new ProblemDetails { Detail = "Ya existe un paciente con este código.", Status = StatusCodes.Status409Conflict });
        }

        paciente.PacienteId = 0;        
        paciente.FechaRegistro = DateTime.UtcNow;
        await patientsRepository.AddAsync(paciente, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { pacienteId = paciente.PacienteId }, paciente);
    }

    [HttpPut("{pacienteId:long}")]
    public async Task<IActionResult> Update(long pacienteId, Paciente paciente, CancellationToken cancellationToken)
    {
        var existing = await patientsRepository.GetByIdAsync(pacienteId, cancellationToken);
        if (existing is null) return NotFound();
        if (await patientsRepository.ExistsAsync(p => p.TipoDocumento == paciente.TipoDocumento && p.NumeroDocumento == paciente.NumeroDocumento && p.PacienteId != pacienteId, cancellationToken: cancellationToken))
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
