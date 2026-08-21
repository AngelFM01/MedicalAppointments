using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Persistence.Configs;

namespace Persistence.Data;

public class AppDbContext : DbContext
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<EmergencyContacts> EmergencyContacts => Set<EmergencyContacts>();


    public AppDbContext()
    {
    }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        /*Configuraracion de las entidades que define como se guardaran
            y organizaran los datos dentro DB*/
        modelBuilder.ApplyConfiguration(new ConfigurationEmergencyContacts());
        modelBuilder.ApplyConfiguration(new PatientConfigType());
    }


}
