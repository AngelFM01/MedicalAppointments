using MedicalAppointments.Core.Interfaces.Repository;
using MedicalAppointments.Domain.Model;
using MediatR;

namespace MedicalAppointments.Core.Feature.Patients.Queries;

public sealed class GetPacienteByIdQuery : IRequest<Paciente?>
{
    public long PacienteId { get; set; }
}

public sealed class GetPacienteByIdQueryHandler(IPatientsRepository patientsRepository) : IRequestHandler<GetPacienteByIdQuery, Paciente?>
{
    public Task<Paciente?> Handle(GetPacienteByIdQuery request, CancellationToken cancellationToken)
        => patientsRepository.GetByIdAsync(request.PacienteId, cancellationToken);
}
