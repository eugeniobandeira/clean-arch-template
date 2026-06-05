using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Features.Samples.Handlers.Delete;
using ErrorOr;

namespace CleanArch.Api.Endpoints.Samples;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/samples/{id:guid}", HandleAsync)
           .WithName("DeleteSample")
           .WithDescription("Delete a sample.")
           .WithTags(Tags.SAMPLE)
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IDeleteSampleHandler handler,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<Deleted> result = await handler.Handle(id, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            errors => errors.ToProblem(httpContext));
    }
}
