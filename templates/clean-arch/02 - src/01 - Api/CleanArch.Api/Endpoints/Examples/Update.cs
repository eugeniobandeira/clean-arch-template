using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Features.Examples.Handlers.Update;
using CleanArch.Application.Features.Examples.Handlers.Update.Request;
using CleanArch.Application.Features.Examples.Mapper;
using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Api.Endpoints.Examples;

internal sealed class Update : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/examples/{id:guid}", HandleAsync)
           .WithName("UpdateExample")
           .WithDescription("Update an existing example.")
           .WithTags(Tags.SAMPLE)
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateExampleRequest request,
        IUpdateExampleHandler handler,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        UpdateExampleCommand command = new(id, request);
        ErrorOr<ExampleEntity> result = await handler.Handle(command, cancellationToken);

        return result.Match(
            entity => Results.Ok(ExampleMapper.ToResponse(entity)),
            errors => errors.ToProblem(httpContext));
    }
}
