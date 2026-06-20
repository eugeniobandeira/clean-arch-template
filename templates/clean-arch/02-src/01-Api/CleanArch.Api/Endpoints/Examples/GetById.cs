using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Common.Handler;
using CleanArch.Application.Features.Examples.Mapper;
using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Api.Endpoints.Examples;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/examples/{id:guid}", HandleAsync)
           .WithName("GetExampleById")
           .WithDescription("Get a example by id.")
           .WithTags(Tags.EXAMPLE);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        IHandler<Guid, ExampleEntity> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<ExampleEntity> result = await handler.Handle(id, cancellationToken);

        return result.Match(
            entity => Results.Ok(ExampleMapper.ToResponse(entity)),
            errors => errors.ToProblem(httpContext));
    }
}
