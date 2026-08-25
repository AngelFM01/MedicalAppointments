using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;

namespace Core.Feature.Patients.Commands;

public sealed class UpdatePacienteCommand : IRequest<bool>
{
    public long PacienteId { get; set; }
    public string CodigoPaciente { get; set; } = null!;
    public string TipoDocumento { get; set; } = null!;
    public string NumeroDocumento { get; set; } = null!;
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public DateOnly FechaNacimiento { get; set; }
    public string Sexo { get; set; } = null!;
    public string? EstadoCivil { get; set; }
    public string? Telefono { get; set; }
    public string? TelefonoSecundario { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
    public string? Ocupacion { get; set; }
    public string? TipoSangre { get; set; }
    public bool Activo { get; set; } = true;
}

public sealed class UpdatePacienteCommandHandler(IPatientsRepository patientsRepository) : IRequestHandler<UpdatePacienteCommand, bool>
{
    public async Task<bool> Handle(UpdatePacienteCommand request, CancellationToken cancellationToken)
    {
        var existing = await patientsRepository.GetByIdAsync(request.PacienteId, cancellationToken);
        if (existing is null) return false;

        if (await patientsRepository.ExistsByDocumentAsync(request.TipoDocumento, request.NumeroDocumento, request.PacienteId, cancellationToken))
            throw new InvalidOperationException("Ya existe un paciente con este tipo y número de documento.");

        var paciente = new Paciente
        {
            PacienteId = request.PacienteId,
            CodigoPaciente = request.CodigoPaciente,
            TipoDocumento = request.TipoDocumento,
            NumeroDocumento = request.NumeroDocumento,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            FechaNacimiento = request.FechaNacimiento,
            Sexo = request.Sexo,
            EstadoCivil = request.EstadoCivil,
            Telefono = request.Telefono,
            TelefonoSecundario = request.TelefonoSecundario,
            Email = request.Email,
            Direccion = request.Direccion,
            Ciudad = request.Ciudad,
            Pais = request.Pais,
            Ocupacion = request.Ocupacion,
            TipoSangre = request.TipoSangre,
            Activo = request.Activo,
            FechaRegistro = existing.FechaRegistro
        };

        await patientsRepository.UpdateAsync(paciente, cancellationToken);
        return true;
    }
}
