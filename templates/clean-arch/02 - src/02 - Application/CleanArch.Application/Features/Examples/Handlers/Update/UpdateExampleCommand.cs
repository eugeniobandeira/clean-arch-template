using CleanArch.Application.Features.Examples.Handlers.Update.Request;

namespace CleanArch.Application.Features.Examples.Handlers.Update;

public sealed record UpdateExampleCommand(Guid Id, UpdateExampleRequest Dto);
