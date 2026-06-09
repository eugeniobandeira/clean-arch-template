using CleanArch.Application.Features.Examples.Handlers.GetAll.Request;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.IoC;

internal static class InfrastructureDependencyInjection
{
    internal static IServiceCollection Register(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
#pragma warning disable S125
        // Register your repository implementations below, following this pattern:
        //
        //   services.AddScoped<YourRepository>();
        //   services.AddScoped<IAddRepository<YourEntity>>(sp => sp.GetRequiredService<YourRepository>());
        //   services.AddScoped<IGetByIdRepository<YourEntity>>(sp => sp.GetRequiredService<YourRepository>());
        //   services.AddScoped<IUpdateRepository<YourEntity>>(sp => sp.GetRequiredService<YourRepository>());
        //   services.AddScoped<IGetAllRepository<YourEntity, YourGetAllRequest>>(sp => sp.GetRequiredService<YourRepository>());
        //   services.AddScoped<IDeleteRepository<YourEntity>>(sp => sp.GetRequiredService<YourRepository>());
#pragma warning restore S125

        services.AddScoped<ExampleRepository>();
        services.AddScoped<IAddRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
        services.AddScoped<IGetByIdRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
        services.AddScoped<IUpdateRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
        services.AddScoped<IGetAllRepository<ExampleEntity, GetAllExampleRequest>>(sp => sp.GetRequiredService<ExampleRepository>());
        services.AddScoped<IDeleteRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());

        return services;
    }
}
