using CleanArch.Application.Features.Examples.Handlers.Create.Request;
using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Application.Features.Examples.Handlers.Create;

public interface ICreateExampleHandler
{
    Task<ErrorOr<ExampleEntity>> Handle(CreateExampleRequest request, CancellationToken cancellationToken = default);
}
