using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Common.Handler;
using CleanArch.Application.Features.Examples.Handlers.Update.Request;
using CleanArch.Application.Features.Examples.Mapper;
using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Api.Endpoints.Examples;

internal sealed class Update : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/examples/{id:guid}", HandleAsync)
           .WithName("UpdateExample")
           .WithDescription("Update an existing example.")
           .WithTags(Tags.EXAMPLE);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateExampleRequest request,
        IHandler<UpdateExampleRequest, ExampleEntity> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<ExampleEntity> result = await handler.Handle(request with { Id = id }, cancellationToken);

        return result.Match(
            entity => Results.Ok(ExampleMapper.ToResponse(entity)),
            errors => errors.ToProblem(httpContext));
    }
}
