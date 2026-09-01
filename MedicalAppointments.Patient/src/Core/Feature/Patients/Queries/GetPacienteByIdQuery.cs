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
    public class GetPacienteByIdQuery : IRequest<Paciente?>
    {
        public long PacienteId { get; set; }
    }

    public class GetPacienteByIdQueryHandler(IPatientsRepository patientsRepository) : IRequestHandler<GetPacienteByIdQuery, Paciente?>
    {
        public Task<Paciente?> Handle(GetPacienteByIdQuery request, CancellationToken cancellationToken)
            => patientsRepository.GetByIdAsync(request.PacienteId, cancellationToken);
    }
}
