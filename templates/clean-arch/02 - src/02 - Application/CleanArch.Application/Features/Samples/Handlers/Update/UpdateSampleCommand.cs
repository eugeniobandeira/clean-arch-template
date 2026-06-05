using CleanArch.Application.Features.Samples.Handlers.Update.Request;

namespace CleanArch.Application.Features.Samples.Handlers.Update;

public sealed record UpdateSampleCommand(Guid Id, UpdateSampleRequest Dto);
