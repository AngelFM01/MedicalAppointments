using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Feature.EmergencyContacts.Queries
{
    public class GetAllEmergencyContactsQuery : IRequest<IReadOnlyList<ContactoEmergencia>> { }

    public class GetAllEmergencyContactsQueryHandler(IRepository<ContactoEmergencia> contactsRepository) : IRequestHandler<GetAllEmergencyContactsQuery, IReadOnlyList<ContactoEmergencia>>
    {
        public Task<IReadOnlyList<ContactoEmergencia>> Handle(GetAllEmergencyContactsQuery request, CancellationToken cancellationToken)
            => contactsRepository.GetAllAsync(cancellationToken);
    }

    public class GetEmergencyContactsByPacienteQuery : IRequest<IReadOnlyList<ContactoEmergencia>>
    {
        public long PacienteId { get; set; }
    }

    public class GetEmergencyContactsByPacienteQueryHandler(
        IRepository<Paciente> patientsRepository,
        IRepository<ContactoEmergencia> contactsRepository) : IRequestHandler<GetEmergencyContactsByPacienteQuery, IReadOnlyList<ContactoEmergencia>>
    {
        public async Task<IReadOnlyList<ContactoEmergencia>> Handle(GetEmergencyContactsByPacienteQuery request, CancellationToken cancellationToken)
        {
            if (await patientsRepository.GetByIdAsync(request.PacienteId, cancellationToken) is null)
                throw new KeyNotFoundException($"Paciente {request.PacienteId} no encontrado.");
            return await contactsRepository.GetByFilterAsync(c => c.PacienteId == request.PacienteId, cancellationToken: cancellationToken);
        }
    }
}
