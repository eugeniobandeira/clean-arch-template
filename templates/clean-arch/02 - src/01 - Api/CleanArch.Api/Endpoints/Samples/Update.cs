using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Features.Samples.Handlers.Update;
using CleanArch.Application.Features.Samples.Handlers.Update.Request;
using CleanArch.Application.Features.Samples.Mapper;
using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Api.Endpoints.Samples;

internal sealed class Update : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/samples/{id:guid}", HandleAsync)
           .WithName("UpdateSample")
           .WithDescription("Update an existing sample.")
           .WithTags(Tags.SAMPLE)
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateSampleRequest request,
        IUpdateSampleHandler handler,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        UpdateSampleCommand command = new(id, request);
        ErrorOr<SampleEntity> result = await handler.Handle(command, cancellationToken);

        return result.Match(
            entity => Results.Ok(SampleMapper.ToResponse(entity)),
            errors => errors.ToProblem(httpContext));
    }
}
