using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Features.Samples.Handlers.Create;
using CleanArch.Application.Features.Samples.Handlers.Create.Request;
using CleanArch.Application.Features.Samples.Mapper;
using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Api.Endpoints.Samples;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/samples", HandleAsync)
           .WithName("CreateSample")
           .WithDescription("Create a new sample.")
           .WithTags(Tags.SAMPLE)
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        CreateSampleRequest request,
        ICreateSampleHandler handler,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<SampleEntity> result = await handler.Handle(request, cancellationToken);

        return result.Match(
            entity => Results.Created($"/api/v1/samples/{entity.Id}", SampleMapper.ToResponse(entity)),
            errors => errors.ToProblem(httpContext));
    }
}
