// Archivo de ejemplo — no compilar. La implementación real está en PatientsRepository.cs.
// Se deja comentado para evitar el error CS0101 (clase duplicada en el mismo namespace).
//
//using MedicalAppointments.Core.Interfaces.Repository;
//using MedicalAppointments.Domain.Model;
//using MedicalAppointments.Persistence.Data;
//using Nuget_Persistence.Abstractions;
//using Nuget_Persistence.Implementations;
//
//namespace MedicalAppointments.Persistence.Repositories;
//
///**/
//internal class PatientsRepository : Repository<Paciente>, IPatientsRepository
//{
//
//    private readonly AppDbContext _context;
//    public PatientsRepository(AppDbContext context) : base(context)
//    {
//        _context = context;
//    }
//
//
//}
