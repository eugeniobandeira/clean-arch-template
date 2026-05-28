using CleanArch.Application.Features.Samples;
using CleanArch.Application.Features.Samples.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.Api.Endpoints.Samples;

public static class SampleEndpoints
{
    public static IEndpointRouteBuilder MapSampleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/samples")
            .WithTags("Samples")
            .RequireAuthorization();

        group.MapGet("/", GetAll)
            .WithName("GetAllSamples")
            .WithSummary("Retorna todos os samples ativos");

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetSampleById")
            .WithSummary("Retorna um sample por ID");

        group.MapPost("/", Create)
            .WithName("CreateSample")
            .WithSummary("Cria um novo sample");

        group.MapPut("/{id:guid}", Update)
            .WithName("UpdateSample")
            .WithSummary("Atualiza um sample existente");

        group.MapDelete("/{id:guid}", Delete)
            .WithName("DeleteSample")
            .WithSummary("Remove um sample");

        return app;
    }

    private static async Task<IResult> GetAll(
        ISampleService service,
        CancellationToken cancellationToken)
    {
        var samples = await service.GetAllAsync(cancellationToken);
        return Results.Ok(samples);
    }

    private static async Task<IResult> GetById(
        Guid id,
        ISampleService service,
        CancellationToken cancellationToken)
    {
        var sample = await service.GetByIdAsync(id, cancellationToken);
        return sample is null ? Results.NotFound() : Results.Ok(sample);
    }

    private static async Task<IResult> Create(
        [FromBody] CreateSampleRequest request,
        ISampleService service,
        IValidator<CreateSampleRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var sample = await service.CreateAsync(request, cancellationToken);
        return Results.CreatedAtRoute("GetSampleById", new { id = sample.Id }, sample);
    }

    private static async Task<IResult> Update(
        Guid id,
        [FromBody] CreateSampleRequest request,
        ISampleService service,
        IValidator<CreateSampleRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        try
        {
            var sample = await service.UpdateAsync(id, request, cancellationToken);
            return Results.Ok(sample);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> Delete(
        Guid id,
        ISampleService service,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return Results.NoContent();
    }
}
