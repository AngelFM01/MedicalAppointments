using Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;
using Persistence.Repositories;

namespace Persistence;

public static class Extension
{
    // Firma idéntica a Estructura: sin parámetro IConfiguration, resuelve configuración internamente
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        IConfiguration configuration;
        using (ServiceProvider provider = services.BuildServiceProvider())
            configuration = provider.GetRequiredService<IConfiguration>();

        var connectionString = configuration["sql:cx"]
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'sql:cx'.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IPatientsRepository, PatientsRepository>();
        services.AddScoped<IContactosEmergenciaRepository, ContactosEmergenciaRepository>();
        return services;
    }

    // Sobrecarga compatibilidad si Program pasa IConfiguration explícitamente
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["sql:cx"]
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'sql:cx'.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IPatientsRepository, PatientsRepository>();
        services.AddScoped<IContactosEmergenciaRepository, ContactosEmergenciaRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(RepositoryGeneric<>));
        return services;
    }
}
