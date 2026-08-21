using Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;
using Persistence.Repositories;

namespace Persistence;

public static class Extension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["sql:cx"]
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'sql:cx'.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IPatients, PatientRepository>();
        services.AddScoped<IEmergencyContactsRepository, EmergencyContactsRepository>();
        return services;
    }
}
