using Core.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Feature.EmergencyContacts.Commands
{
    public class UpdateEmergencyContactCommand : IRequest<bool>
    {
        public long PacienteId { get; set; }
        public long ContactoEmergenciaId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string? Parentesco { get; set; }
        public string Telefono { get; set; } = null!;
        public string? TelefonoSecundario { get; set; }
        public string? Email { get; set; }
        public int Prioridad { get; set; } = 1;
        public bool Activo { get; set; } = true;
    }

    public class UpdateEmergencyContactCommandHandler(IContactosEmergenciaRepository contactsRepository) : IRequestHandler<UpdateEmergencyContactCommand, bool>
    {
        public async Task<bool> Handle(UpdateEmergencyContactCommand request, CancellationToken cancellationToken)
        {
            var existing = await contactsRepository.GetByIdAsync(request.ContactoEmergenciaId, cancellationToken);
            if (existing is null || existing.PacienteId != request.PacienteId) return false;

            var contacto = new Domain.Model.ContactoEmergencia
            {
                ContactoEmergenciaId = request.ContactoEmergenciaId,
                PacienteId = request.PacienteId,
                NombreCompleto = request.NombreCompleto,
                Parentesco = request.Parentesco,
                Telefono = request.Telefono,
                TelefonoSecundario = request.TelefonoSecundario,
                Email = request.Email,
                Prioridad = request.Prioridad,
                Activo = request.Activo
            };

            await contactsRepository.UpdateAsync(contacto, cancellationToken);
            return true;
        }
    }
}
