using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Application.Features.Examples.Handlers.GetById;

public interface IGetByIdExampleHandler
{
    Task<ErrorOr<ExampleEntity>> Handle(Guid id, CancellationToken cancellationToken = default);
}
