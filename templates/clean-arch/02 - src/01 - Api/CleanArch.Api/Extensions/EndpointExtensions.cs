using CleanArch.Api.Abstract;
using System.Reflection;

namespace CleanArch.Api.Extensions;

internal static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> endpointTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                        && t.IsAssignableTo(typeof(IEndpoint)));

        foreach (Type? type in endpointTypes)
            services.AddTransient(typeof(IEndpoint), type);

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        IEnumerable<IEndpoint> endpoints = app.ServiceProvider
            .GetServices<IEndpoint>();

        foreach (IEndpoint endpoint in endpoints)
            endpoint.MapEndpoint(app);

        return app;
    }
}
