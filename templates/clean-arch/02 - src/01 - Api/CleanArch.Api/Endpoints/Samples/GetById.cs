using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Features.Samples.Handlers.GetById;
using CleanArch.Application.Features.Samples.Mapper;
using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Api.Endpoints.Samples;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/samples/{id:guid}", HandleAsync)
           .WithName("GetSampleById")
           .WithDescription("Get a sample by id.")
           .WithTags(Tags.SAMPLE)
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IGetByIdSampleHandler handler,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<SampleEntity> result = await handler.Handle(id, cancellationToken);

        return result.Match(
            entity => Results.Ok(SampleMapper.ToResponse(entity)),
            errors => errors.ToProblem(httpContext));
    }
}
