using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Feature.Patients.Queries
{
    public class GetPacientesQuery : IRequest<IReadOnlyList<Paciente>> { }

    public class GetPacientesQueryHandler : IRequestHandler<GetPacientesQuery, IReadOnlyList<Paciente>>
    {
        private readonly IPatientsRepository _pacienteRepository;

        public GetPacientesQueryHandler(IPatientsRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<IReadOnlyList<Paciente>> Handle(GetPacientesQuery request, CancellationToken cancellationToken)
        {
            var all = await _pacienteRepository.GetAllAsync(cancellationToken);
            return all.Where(p => p.Activo).ToList();
        }
    }
}
