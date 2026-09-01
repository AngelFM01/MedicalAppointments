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
        Ok(await patientsRepository.GetByFilterAsync(p => p.Activo == true, cancellationToken: cancellationToken));

    [HttpGet("{pacienteId:long}")]
    public async Task<ActionResult<Paciente>> GetById(long pacienteId, CancellationToken cancellationToken)
    {
        var paciente = await patientsRepository.GetByIdAsync(pacienteId, cancellationToken);
        if (paciente == null || !await patientsRepository.ExistsAsync(p => p.PacienteId == pacienteId, cancellationToken))
            return NotFound();
        return Ok(paciente);
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
        // 1. Obtener la entidad existente (ya rastreada por el contexto)
        var existing = await patientsRepository.GetByIdAsync(pacienteId, cancellationToken);
        if (existing is null)
            return NotFound();

        // 2. Validar duplicado de documento (excluyendo el propio registro)
        if (await patientsRepository.ExistsAsync(p => p.TipoDocumento == paciente.TipoDocumento
                                                      && p.NumeroDocumento == paciente.NumeroDocumento
                                                      && p.PacienteId != pacienteId, cancellationToken))
            return Conflict(new ProblemDetails
            {
                Detail = "Ya existe un paciente con este tipo y número de documento.",
                Status = StatusCodes.Status409Conflict
            });

        // 3. Mapear los valores del DTO (o entidad recibida) al objeto rastreado
        existing.CodigoPaciente = paciente.CodigoPaciente;
        existing.TipoDocumento = paciente.TipoDocumento;
        existing.NumeroDocumento = paciente.NumeroDocumento;
        existing.Nombres = paciente.Nombres;
        existing.Apellidos = paciente.Apellidos;
        existing.FechaNacimiento = paciente.FechaNacimiento;
        existing.Sexo = paciente.Sexo;
        existing.EstadoCivil = paciente.EstadoCivil;
        existing.Telefono = paciente.Telefono;
        existing.TelefonoSecundario = paciente.TelefonoSecundario;
        existing.Email = paciente.Email;
        existing.Direccion = paciente.Direccion;
        existing.Ciudad = paciente.Ciudad;
        existing.Pais = paciente.Pais;
        existing.Ocupacion = paciente.Ocupacion;
        existing.TipoSangre = paciente.TipoSangre;
        existing.Activo = paciente.Activo;
        

        // 4. Guardar cambios (el contexto ya rastrea `existing`)
        await patientsRepository.UpdateAsync(existing, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{pacienteId:long}")]
    public async Task<IActionResult> Delete(long pacienteId, CancellationToken cancellationToken)
    {
        
        var paciente = await patientsRepository.GetByIdAsync(pacienteId, cancellationToken);
        if (paciente is null)
            return NotFound();

        // 2. Soft Delete: marcar como inactivo
        paciente.Activo = false;        
        await patientsRepository.UpdateAsync(paciente, cancellationToken);

        return NoContent();
    }


}
