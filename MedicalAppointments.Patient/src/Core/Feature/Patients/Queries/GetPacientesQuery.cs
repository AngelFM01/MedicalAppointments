using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;

namespace Core.Feature.Patients.Queries;

public sealed class GetPacientesQuery : IRequest<IReadOnlyList<Paciente>> { }

public sealed class GetPacientesQueryHandler(IPatientsRepository patientsRepository) : IRequestHandler<GetPacientesQuery, IReadOnlyList<Paciente>>
{
    public Task<IReadOnlyList<Paciente>> Handle(GetPacientesQuery request, CancellationToken cancellationToken)
        => patientsRepository.GetAllAsync(cancellationToken);
}
