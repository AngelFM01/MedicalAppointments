using MedicalAppointments.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Persistence.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<ContactoEmergencia> ContactosEmergencia => Set<ContactoEmergencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
