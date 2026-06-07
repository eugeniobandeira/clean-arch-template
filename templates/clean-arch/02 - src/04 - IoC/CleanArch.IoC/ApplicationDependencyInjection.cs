using CleanArch.Application.Common.Commands;
using CleanArch.Application.Common.Handler;
using CleanArch.Application.Features.Examples.Handlers.Create;
using CleanArch.Application.Features.Examples.Handlers.Create.Request;
using CleanArch.Application.Features.Examples.Handlers.Delete;
using CleanArch.Application.Features.Examples.Handlers.GetAll;
using CleanArch.Application.Features.Examples.Handlers.GetAll.Request;
using CleanArch.Application.Features.Examples.Handlers.GetById;
using CleanArch.Application.Features.Examples.Handlers.Update;
using CleanArch.Application.Features.Examples.Handlers.Update.Request;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using ErrorOr;
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

        services.AddScoped<IHandler<CreateExampleRequest, ExampleEntity>, CreateExampleHandler>();
        services.AddScoped<IHandler<Guid, ExampleEntity>, GetByIdExampleHandler>();
        services.AddScoped<IHandler<GetAllExampleRequest, PagedResult<ExampleEntity>>, GetAllExampleHandler>();
        services.AddScoped<IHandler<UpdateCommand<UpdateExampleRequest>, ExampleEntity>, UpdateExampleHandler>();
        services.AddScoped<IHandler<Guid, Deleted>, DeleteExampleHandler>();

        return services;
    }
}
