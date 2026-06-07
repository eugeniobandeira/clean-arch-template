using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Application.Features.Examples.Handlers.Update;

public interface IUpdateExampleHandler
{
    Task<ErrorOr<ExampleEntity>> Handle(UpdateExampleCommand command, CancellationToken cancellationToken = default);
}
