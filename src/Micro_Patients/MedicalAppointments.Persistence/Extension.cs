using MedicalAppointments.Core.Interfaces.Repository;
using MedicalAppointments.Persistence.Data;
using MedicalAppointments.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.Persistence;

public static class Extension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        IConfiguration configuration;
        using (ServiceProvider provider = services.BuildServiceProvider())
            configuration = provider.GetRequiredService<IConfiguration>();

        return services.AddPersistence(configuration);
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["sql:cx"]
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'sql:cx'.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IPatientsRepository, PatientsRepository>();
        services.AddScoped<IContactosEmergenciaRepository, ContactosEmergenciaRepository>();

        return services;
    }
}
