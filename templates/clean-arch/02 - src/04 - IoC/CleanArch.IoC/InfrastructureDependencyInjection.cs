using Amazon.DynamoDBv2;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Domain.Interfaces.UnitOfWork;
using CleanArch.Infrastructure.DataAccess;
using CleanArch.Infrastructure.Persistence.DynamoDB;
using CleanArch.Infrastructure.Persistence.EfCore;
using CleanArch.Infrastructure.Repositories.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.IoC;

internal static class InfrastructureDependencyInjection
{
    internal static IServiceCollection Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEfCore(configuration);
        services.AddDynamoDB();
        services.AddRepositories();
        services.AddUnitOfWork();

        return services;
    }

    private static IServiceCollection AddEfCore(this IServiceCollection services, IConfiguration configuration)
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
        // Para usar DynamoDB, troque SampleEfRepository por SampleDynamoRepository nas linhas abaixo.
        // ATENÇÃO: DynamoDB não suporta transações compartilhadas com IUnitOfWork — ver comentário em SampleDynamoRepository.
        services.AddScoped<SampleEfRepository>();

        services.AddScoped<IAddRepository<SampleEntity>>(sp => sp.GetRequiredService<SampleEfRepository>());
        services.AddScoped<IGetByIdRepository<SampleEntity>>(sp => sp.GetRequiredService<SampleEfRepository>());
        services.AddScoped<IUpdateRepository<SampleEntity>>(sp => sp.GetRequiredService<SampleEfRepository>());
        services.AddScoped<IGetAllRepository<SampleEntity, SampleFilter>>(sp => sp.GetRequiredService<SampleEfRepository>());
        services.AddScoped<IDeleteRepository<SampleEntity>>(sp => sp.GetRequiredService<SampleEfRepository>());

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<UnitOfWork>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UnitOfWork>());
        services.AddScoped<ITransactionUnitOfWork>(sp => sp.GetRequiredService<UnitOfWork>());

        return services;
    }
}
