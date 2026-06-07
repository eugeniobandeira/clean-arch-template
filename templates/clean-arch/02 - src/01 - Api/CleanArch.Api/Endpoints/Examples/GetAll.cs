using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Common.Mapper;
using CleanArch.Application.Features.Examples.Handlers.GetAll;
using CleanArch.Application.Features.Examples.Mapper;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.Api.Endpoints.Examples;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/examples", HandleAsync)
           .WithName("GetAllExamples")
           .WithDescription("Get all active examples.")
           .WithTags(Tags.SAMPLE)
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        IGetAllExampleHandler handler,
        HttpContext httpContext,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        ExampleFilter filter = new(Page: page, PageSize: pageSize);

        ErrorOr<PagedResult<ExampleEntity>> result = await handler.Handle(filter, cancellationToken);

        return result.Match(
            pagedResult => Results.Ok(ApiListResponseMapper.ToListResponse(
                [.. pagedResult.Items.Select(ExampleMapper.ToResponse)],
                pagedResult.Total,
                filter.Page,
                filter.PageSize)),
            errors => errors.ToProblem(httpContext));
    }
}
