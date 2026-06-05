using CleanArch.Application.Features.Samples.Handlers.Create;
using CleanArch.Application.Features.Samples.Handlers.Delete;
using CleanArch.Application.Features.Samples.Handlers.GetAll;
using CleanArch.Application.Features.Samples.Handlers.GetById;
using CleanArch.Application.Features.Samples.Handlers.Update;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleanArch.IoC;

internal static class ApplicationDependencyInjection
{
    internal static IServiceCollection Register(IServiceCollection services)
    {
        Assembly assembly = typeof(CreateSampleHandler).Assembly;
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ICreateSampleHandler, CreateSampleHandler>();
        services.AddScoped<IGetByIdSampleHandler, GetByIdSampleHandler>();
        services.AddScoped<IGetAllSampleHandler, GetAllSampleHandler>();
        services.AddScoped<IUpdateSampleHandler, UpdateSampleHandler>();
        services.AddScoped<IDeleteSampleHandler, DeleteSampleHandler>();

        return services;
    }
}
