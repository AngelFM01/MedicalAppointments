using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Feature.Patients.Queries
{
    public class GetPacientesQuery : IRequest<IReadOnlyList<Paciente>> { }

    public class GetPacientesQueryHandler(IPatientsRepository patientsRepository) : IRequestHandler<GetPacientesQuery, IReadOnlyList<Paciente>>
    {
        public Task<IReadOnlyList<Paciente>> Handle(GetPacientesQuery request, CancellationToken cancellationToken)
            => patientsRepository.GetAllAsync(cancellationToken);
    }
}
