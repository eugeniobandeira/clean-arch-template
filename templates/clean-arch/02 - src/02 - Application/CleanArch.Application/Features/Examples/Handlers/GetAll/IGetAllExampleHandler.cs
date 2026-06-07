using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using ErrorOr;

namespace CleanArch.Application.Features.Examples.Handlers.GetAll;

public interface IGetAllExampleHandler
{
    Task<ErrorOr<PagedResult<ExampleEntity>>> Handle(ExampleFilter filter, CancellationToken cancellationToken = default);
}
