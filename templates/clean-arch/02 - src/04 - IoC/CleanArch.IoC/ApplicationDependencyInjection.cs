using CleanArch.Application.Features.Examples.Handlers.Create;
using CleanArch.Application.Features.Examples.Handlers.Delete;
using CleanArch.Application.Features.Examples.Handlers.GetAll;
using CleanArch.Application.Features.Examples.Handlers.GetById;
using CleanArch.Application.Features.Examples.Handlers.Update;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleanArch.IoC;

internal static class ApplicationDependencyInjection
{
    internal static IServiceCollection Register(IServiceCollection services)
    {
        Assembly assembly = typeof(CreateExampleHandler).Assembly;
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ICreateExampleHandler, CreateExampleHandler>();
        services.AddScoped<IGetByIdExampleHandler, GetByIdExampleHandler>();
        services.AddScoped<IGetAllExampleHandler, GetAllExampleHandler>();
        services.AddScoped<IUpdateExampleHandler, UpdateExampleHandler>();
        services.AddScoped<IDeleteExampleHandler, DeleteExampleHandler>();

        return services;
    }
}
