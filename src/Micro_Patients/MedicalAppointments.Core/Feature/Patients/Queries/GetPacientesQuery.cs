using MedicalAppointments.Core.Interfaces.Repository;
using MedicalAppointments.Domain.Model;
using MediatR;

namespace MedicalAppointments.Core.Feature.Patients.Queries;

public sealed class GetPacientesQuery : IRequest<IReadOnlyList<Paciente>> { }

public sealed class GetPacientesQueryHandler(IPatientsRepository patientsRepository) : IRequestHandler<GetPacientesQuery, IReadOnlyList<Paciente>>
{
    public Task<IReadOnlyList<Paciente>> Handle(GetPacientesQuery request, CancellationToken cancellationToken)
        => patientsRepository.GetAllAsync(cancellationToken);
}
