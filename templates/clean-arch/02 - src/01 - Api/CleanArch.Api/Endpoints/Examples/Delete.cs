using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Features.Examples.Handlers.Delete;
using ErrorOr;

namespace CleanArch.Api.Endpoints.Examples;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/examples/{id:guid}", HandleAsync)
           .WithName("DeleteExample")
           .WithDescription("Delete a example.")
           .WithTags(Tags.SAMPLE)
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IDeleteExampleHandler handler,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<Deleted> result = await handler.Handle(id, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            errors => errors.ToProblem(httpContext));
    }
}
