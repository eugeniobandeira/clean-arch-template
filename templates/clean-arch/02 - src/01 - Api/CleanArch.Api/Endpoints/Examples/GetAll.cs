using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Common.Handler;
using CleanArch.Application.Common.Mapper;
using CleanArch.Application.Features.Examples.Handlers.GetAll.Request;
using CleanArch.Application.Features.Examples.Mapper;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Api.Endpoints.Examples;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/examples", HandleAsync)
           .WithName("GetAllExamples")
           .WithDescription("Get all active examples.")
           .WithTags(Tags.EXAMPLE)
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetAllExampleRequest request,
        IHandler<GetAllExampleRequest, PagedResult<ExampleEntity>> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<PagedResult<ExampleEntity>> result = await handler.Handle(request, cancellationToken);

        return result.Match(
            pagedResult => Results.Ok(ApiListResponseMapper.ToListResponse(
                [.. pagedResult.Items.Select(ExampleMapper.ToResponse)],
                pagedResult.Total,
                request.Page,
                request.PageSize)),
            errors => errors.ToProblem(httpContext));
    }
}
