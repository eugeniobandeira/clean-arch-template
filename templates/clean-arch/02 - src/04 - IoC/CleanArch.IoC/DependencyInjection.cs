using Amazon.DynamoDBv2;
using CleanArch.Application.Features.Samples;
using CleanArch.Domain.Repositories;
using CleanArch.Infrastructure.Persistence.DynamoDB;
using CleanArch.Infrastructure.Persistence.EfCore;
using CleanArch.Infrastructure.Repositories.EfCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ISampleService).Assembly;
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEfCore(configuration);
        services.AddDynamoDB();
        services.AddRepositories();
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddEfCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(
                    typeof(AppDbContext).Assembly.FullName)));

        return services;
    }

    private static IServiceCollection AddDynamoDB(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton<DynamoDbContext>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Troque SampleEfRepository por SampleDynamoRepository para usar DynamoDB
        services.AddScoped<ISampleRepository, SampleEfRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISampleService, SampleService>();

        return services;
    }
}
