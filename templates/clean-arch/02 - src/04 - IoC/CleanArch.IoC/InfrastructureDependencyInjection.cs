using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.IoC;

internal static class InfrastructureDependencyInjection
{
    internal static IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
#pragma warning disable S1135, S125
        // TODO: Register your repository implementation here.
        //
        // Each interface maps to a single operation — register them all pointing
        // to the same concrete type using the factory overload, e.g.:
        //
        //   services.AddScoped<ExampleRepository>();
        //   services.AddScoped<IAddRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
        //   services.AddScoped<IGetByIdRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
        //   services.AddScoped<IUpdateRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
        //   services.AddScoped<IGetAllRepository<ExampleEntity, GetAllExampleRequest>>(sp => sp.GetRequiredService<ExampleRepository>());
        //   services.AddScoped<IDeleteRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
#pragma warning restore S1135, S125

        return services;
    }
}
