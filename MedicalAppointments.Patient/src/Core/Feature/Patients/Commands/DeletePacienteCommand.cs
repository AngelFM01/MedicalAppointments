using Core.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Feature.Patients.Commands
{
    public class DeletePacienteCommand : IRequest<bool>
    {
        public long PacienteId { get; set; }
    }

    public class DeletePacienteCommandHandler(IPatientsRepository patientsRepository) : IRequestHandler<DeletePacienteCommand, bool>
    {
        public Task<bool> Handle(DeletePacienteCommand request, CancellationToken cancellationToken)
            => patientsRepository.DeleteAsync(request.PacienteId, cancellationToken);
    }
}
