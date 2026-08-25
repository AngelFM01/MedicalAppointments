using Core.Interfaces.Repository;
using MediatR;

namespace Core.Feature.EmergencyContacts.Commands;

public sealed class DeleteEmergencyContactCommand : IRequest<bool>
{
    public long PacienteId { get; set; }
    public long ContactoEmergenciaId { get; set; }
}

public sealed class DeleteEmergencyContactCommandHandler(IContactosEmergenciaRepository contactsRepository) : IRequestHandler<DeleteEmergencyContactCommand, bool>
{
    public async Task<bool> Handle(DeleteEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var existing = await contactsRepository.GetByIdAsync(request.ContactoEmergenciaId, cancellationToken);
        if (existing is null || existing.PacienteId != request.PacienteId) return false;
        return await contactsRepository.DeleteAsync(request.ContactoEmergenciaId, cancellationToken);
    }
}
