using MedicalAppointments.Core.Interfaces.Repository;
using MediatR;

namespace MedicalAppointments.Core.Feature.Patients.Commands;

public sealed class DeletePacienteCommand : IRequest<bool>
{
    public long PacienteId { get; set; }
}

public sealed class DeletePacienteCommandHandler(IPatientsRepository patientsRepository) : IRequestHandler<DeletePacienteCommand, bool>
{
    public Task<bool> Handle(DeletePacienteCommand request, CancellationToken cancellationToken)
        => patientsRepository.DeleteAsync(request.PacienteId, cancellationToken);
}
