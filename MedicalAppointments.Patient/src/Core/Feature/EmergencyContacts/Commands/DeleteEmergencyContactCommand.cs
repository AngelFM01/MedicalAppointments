using Core.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Feature.EmergencyContacts.Commands
{
    public class DeleteEmergencyContactCommand : IRequest<bool>
    {
        public long PacienteId { get; set; }
        public long ContactoEmergenciaId { get; set; }
    }

    public class DeleteEmergencyContactCommandHandler(IContactosEmergenciaRepository contactsRepository) : IRequestHandler<DeleteEmergencyContactCommand, bool>
    {
        public async Task<bool> Handle(DeleteEmergencyContactCommand request, CancellationToken cancellationToken)
        {
            var existing = await contactsRepository.GetByIdAsync(request.ContactoEmergenciaId, cancellationToken);
            if (existing is null || existing.PacienteId != request.PacienteId) return false;
            return await contactsRepository.DeleteAsync(request.ContactoEmergenciaId, cancellationToken);
        }
    }
}
