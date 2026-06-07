using ErrorOr;

namespace CleanArch.Application.Features.Examples.Handlers.Delete;

public interface IDeleteExampleHandler
{
    Task<ErrorOr<Deleted>> Handle(Guid id, CancellationToken cancellationToken = default);
}
