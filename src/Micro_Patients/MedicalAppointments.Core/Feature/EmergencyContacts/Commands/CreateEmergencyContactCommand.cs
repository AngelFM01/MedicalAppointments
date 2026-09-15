using MedicalAppointments.Core.Interfaces.Repository;
using MedicalAppointments.Domain.Model;
using MediatR;

namespace MedicalAppointments.Core.Feature.EmergencyContacts.Commands;

public sealed class CreateEmergencyContactCommand : IRequest<ContactoEmergencia>
{
    public long PacienteId { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string? Parentesco { get; set; }
    public string Telefono { get; set; } = null!;
    public string? TelefonoSecundario { get; set; }
    public string? Email { get; set; }
    public int Prioridad { get; set; } = 1;
    public bool Activo { get; set; } = true;
}

public sealed class CreateEmergencyContactCommandHandler(
    IPatientsRepository patientsRepository,
    IContactosEmergenciaRepository contactsRepository) : IRequestHandler<CreateEmergencyContactCommand, ContactoEmergencia>
{
    public async Task<ContactoEmergencia> Handle(CreateEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        if (await patientsRepository.GetByIdAsync(request.PacienteId, cancellationToken) is null)
            throw new KeyNotFoundException($"Paciente {request.PacienteId} no encontrado.");

        var contacto = new ContactoEmergencia
        {
            PacienteId = request.PacienteId,
            NombreCompleto = request.NombreCompleto,
            Parentesco = request.Parentesco,
            Telefono = request.Telefono,
            TelefonoSecundario = request.TelefonoSecundario,
            Email = request.Email,
            Prioridad = request.Prioridad,
            Activo = request.Activo
        };

        await contactsRepository.AddAsync(contacto, cancellationToken);
        return contacto;
    }
}
