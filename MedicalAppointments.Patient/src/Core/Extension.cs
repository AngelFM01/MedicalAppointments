using System.Reflection;
using Microsoft.Extensions.DependencyInjection;



namespace Core;

public static class Extension
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}
