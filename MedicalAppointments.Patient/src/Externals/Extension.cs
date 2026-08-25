using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Externals;

public static class Extension
{
    public static IServiceCollection AddExternals(this IServiceCollection services)
    {
        IConfiguration configuration;
        using (ServiceProvider provider = services.BuildServiceProvider())
            configuration = provider.GetRequiredService<IConfiguration>();

        // Aquí se registrarían HttpClients tipados hacia otros microservicios
        // ej: services.AddHttpClient<IPatientServiceClient, PatientServiceClient>(...)

        return services;
    }
}
