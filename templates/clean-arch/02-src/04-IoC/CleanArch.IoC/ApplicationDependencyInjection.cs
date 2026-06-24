using CleanArch.Application.Common.Handler;
using CleanArch.Application.Features.Examples.Handlers.Create;
using CleanArch.Application.Features.Examples.Handlers.Create.Request;
using CleanArch.Application.Features.Examples.Handlers.Create.Validator;
using CleanArch.Application.Features.Examples.Handlers.Delete;
using CleanArch.Application.Features.Examples.Handlers.GetAll;
using CleanArch.Application.Features.Examples.Handlers.GetAll.Request;
using CleanArch.Application.Features.Examples.Handlers.GetById;
using CleanArch.Application.Features.Examples.Handlers.Update;
using CleanArch.Application.Features.Examples.Handlers.Update.Request;
using CleanArch.Application.Features.Examples.Handlers.Update.Validator;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.IoC;

internal static class ApplicationDependencyInjection
{
    internal static IServiceCollection Register(IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateExampleRequest>, CreateExampleValidator>();
        services.AddScoped<IValidator<UpdateExampleRequest>, UpdateExampleValidator>();

        services.AddScoped<IHandler<CreateExampleRequest, ExampleEntity>, CreateExampleHandler>();
        services.AddScoped<IHandler<Guid, Deleted>, DeleteExampleHandler>();
        services.AddScoped<IHandler<GetAllExampleRequest, PagedResult<ExampleEntity>>, GetAllExampleHandler>();
        services.AddScoped<IHandler<Guid, ExampleEntity>, GetByIdExampleHandler>();
        services.AddScoped<IHandler<UpdateExampleRequest, ExampleEntity>, UpdateExampleHandler>();

        return services;
    }
}
