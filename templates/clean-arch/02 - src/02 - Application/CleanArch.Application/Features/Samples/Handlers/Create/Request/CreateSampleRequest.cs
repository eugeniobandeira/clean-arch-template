namespace CleanArch.Application.Features.Samples.Handlers.Create.Request;

public sealed record CreateSampleRequest(
    string Name,
    string Description
);
