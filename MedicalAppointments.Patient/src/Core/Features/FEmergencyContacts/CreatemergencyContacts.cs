using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;

namespace Core.Features.FEmergencyContacts
{
    public class CreatemergencyContacts : IRequest<EmergencyContacts>
    {
        public class CreateEmergencyContactsCommand : IRequest<EmergencyContacts>
        {
            public Guid PacientID { get; set; }
            public string NombreCompleto { get; set; } = null!;
            public string Parentesco { get; set; } = null!;
            public string Telefono { get; set; } = null!;
            public string TelefonoAlternativo { get; set; } = null!;
            public string Email { get; set; } = null!;
            public int Prioridad { get; set; } = 1;
            public bool Activo { get; set; } = true;
            public DateTime fechaCreacion { get; set; }
            public string usuarioCreacion { get; set; } = null!;
        }

        public class CreateEmergencyContactsHandler : IRequestHandler<CreateEmergencyContactsCommand, EmergencyContacts>
        {
            private readonly IEmergencyContactsRepository _emergencyContactsRepository;
            public CreateEmergencyContactsHandler(IEmergencyContactsRepository emergencyContactsRepository)
            {
                _emergencyContactsRepository = emergencyContactsRepository;
            }
            public async Task<EmergencyContacts> Handle(CreateEmergencyContactsCommand request, CancellationToken cancellationToken)
            {
                var emergencyContact = new EmergencyContacts
                {
                    PacientID = request.PacientID,
                    NombreCompleto = request.NombreCompleto,
                    Parentesco = request.Parentesco,
                    Telefono = request.Telefono,
                    TelefonoAlternativo = request.TelefonoAlternativo,
                    Email = request.Email,
                    Prioridad = request.Prioridad,
                    Activo = request.Activo,
                    FechaCreacion = request.fechaCreacion,
                    usuarioCreacion = request.usuarioCreacion
                };
                return await _emergencyContactsRepository.AddContact(emergencyContact);
            }
        }
    }
}
