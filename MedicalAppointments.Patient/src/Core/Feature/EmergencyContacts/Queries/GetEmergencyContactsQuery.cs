using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;

namespace Core.Feature.EmergencyContacts.Queries;

public sealed class GetAllEmergencyContactsQuery : IRequest<IReadOnlyList<ContactoEmergencia>> { }

public sealed class GetAllEmergencyContactsQueryHandler(IContactosEmergenciaRepository contactsRepository) : IRequestHandler<GetAllEmergencyContactsQuery, IReadOnlyList<ContactoEmergencia>>
{
    public Task<IReadOnlyList<ContactoEmergencia>> Handle(GetAllEmergencyContactsQuery request, CancellationToken cancellationToken)
        => contactsRepository.GetAllAsync(cancellationToken);
}

public sealed class GetEmergencyContactsByPacienteQuery : IRequest<IReadOnlyList<ContactoEmergencia>>
{
    public long PacienteId { get; set; }
}

public sealed class GetEmergencyContactsByPacienteQueryHandler(
    IPatientsRepository patientsRepository,
    IContactosEmergenciaRepository contactsRepository) : IRequestHandler<GetEmergencyContactsByPacienteQuery, IReadOnlyList<ContactoEmergencia>>
{
    public async Task<IReadOnlyList<ContactoEmergencia>> Handle(GetEmergencyContactsByPacienteQuery request, CancellationToken cancellationToken)
    {
        if (await patientsRepository.GetByIdAsync(request.PacienteId, cancellationToken) is null)
            throw new KeyNotFoundException($"Paciente {request.PacienteId} no encontrado.");
        return await contactsRepository.GetByPacienteIdAsync(request.PacienteId, cancellationToken);
    }
}
