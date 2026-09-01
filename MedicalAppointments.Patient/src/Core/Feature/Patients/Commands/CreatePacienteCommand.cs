using Core.Interfaces.Repository;
using Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Feature.Patients.Commands
{
    public class CreatePacienteCommand : IRequest<Paciente>
    {
        public string CodigoPaciente { get; set; } = null!;
        public string TipoDocumento { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public DateOnly FechaNacimiento { get; set; }
        public string Sexo { get; set; } = null!;
        public string? EstadoCivil { get; set; }
        public string? Telefono { get; set; }
        public string? TelefonoSecundario { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        public string? Ocupacion { get; set; }
        public string? TipoSangre { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class CreatePacienteCommandHandler : IRequestHandler<CreatePacienteCommand, Paciente>
    {
        private readonly IRepository<Paciente> patientsRepository;

        public CreatePacienteCommandHandler(IRepository<Paciente> patientsRepository)
        {
            this.patientsRepository = patientsRepository;
        }

        public async Task<Paciente> Handle(CreatePacienteCommand request, CancellationToken cancellationToken)
        {
            if (await patientsRepository.ExistsAsync(p => p.TipoDocumento == request.TipoDocumento && p.NumeroDocumento == request.NumeroDocumento, cancellationToken))
                throw new InvalidOperationException("Ya existe un paciente con este tipo y número de documento.");

            

            var paciente = new Paciente
            {
                CodigoPaciente = request.CodigoPaciente,
                TipoDocumento = request.TipoDocumento,
                NumeroDocumento = request.NumeroDocumento,
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                FechaNacimiento = request.FechaNacimiento,
                Sexo = request.Sexo,
                EstadoCivil = request.EstadoCivil,
                Telefono = request.Telefono,
                TelefonoSecundario = request.TelefonoSecundario,
                Email = request.Email,
                Direccion = request.Direccion,
                Ciudad = request.Ciudad,
                Pais = request.Pais,
                Ocupacion = request.Ocupacion,
                TipoSangre = request.TipoSangre,
                Activo = request.Activo
            };
           

            await patientsRepository.AddAsync(paciente, cancellationToken);
            return paciente;
        }
    }
}
