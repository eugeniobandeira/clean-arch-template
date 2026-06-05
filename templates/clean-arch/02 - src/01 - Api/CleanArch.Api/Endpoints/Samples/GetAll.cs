using CleanArch.Api.Abstract;
using CleanArch.Api.Extensions;
using CleanArch.Application.Common.Mapper;
using CleanArch.Application.Features.Samples.Handlers.GetAll;
using CleanArch.Application.Features.Samples.Mapper;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.Api.Endpoints.Samples;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/samples", HandleAsync)
           .WithName("GetAllSamples")
           .WithDescription("Get all active samples.")
           .WithTags(Tags.SAMPLE)
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        IGetAllSampleHandler handler,
        HttpContext httpContext,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        SampleFilter filter = new(Page: page, PageSize: pageSize);

        ErrorOr<PagedResult<SampleEntity>> result = await handler.Handle(filter, cancellationToken);

        return result.Match(
            pagedResult => Results.Ok(ApiListResponseMapper.ToListResponse(
                [.. pagedResult.Items.Select(SampleMapper.ToResponse)],
                pagedResult.Total,
                filter.Page,
                filter.PageSize)),
            errors => errors.ToProblem(httpContext));
    }
}
