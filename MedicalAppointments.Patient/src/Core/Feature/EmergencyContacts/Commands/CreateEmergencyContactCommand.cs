using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Feature.EmergencyContacts.Commands
{
    public class CreateEmergencyContactCommand : IRequest<ContactoEmergencia>
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

    public class CreateEmergencyContactCommandHandler : IRequestHandler<CreateEmergencyContactCommand, ContactoEmergencia>
    {
        private readonly IRepository<ContactoEmergencia> _repositoryEmergency;
        private readonly IRepository<Paciente> _repositoryPaciente;

        public CreateEmergencyContactCommandHandler(IRepository<ContactoEmergencia> contactsRepository, IRepository<Paciente> patientsRepository)
        {
            this._repositoryEmergency = contactsRepository;
            this._repositoryPaciente = patientsRepository;
        }

        public async Task<ContactoEmergencia> Handle(CreateEmergencyContactCommand request, CancellationToken cancellationToken)
        {
            if (await _repositoryPaciente.GetByIdAsync(request.PacienteId, cancellationToken) is null)
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

            await _repositoryEmergency.AddAsync(contacto, cancellationToken);
            return contacto;
        }
    }
}
