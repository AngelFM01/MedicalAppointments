using Core.Interfaces.Persistence;
using Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;
using Persistence.Generic;
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

        return services.AddPersistence(configuration);
    }

    // Sobrecarga compatibilidad si Program pasa IConfiguration explícitamente
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["sql:cx"]
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'sql:cx'.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        // GenericRepository<,> (biblioteca Persistence.Generic) depende de DbContext, no de AppDbContext,
        // para poder reutilizarse con cualquier contexto EF Core. Se expone aquí el mismo AppDbContext como DbContext.
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Patrón genérico: una sola implementación (Persistence.Generic) para el CRUD de cualquier entidad IEntity<TKey>.
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

        // Repositorios concretos (heredan del genérico y añaden consultas específicas).
        services.AddScoped<IPatientsRepository, PatientsRepository>();
        services.AddScoped<IContactosEmergenciaRepository, ContactosEmergenciaRepository>();

        return services;
    }
}
