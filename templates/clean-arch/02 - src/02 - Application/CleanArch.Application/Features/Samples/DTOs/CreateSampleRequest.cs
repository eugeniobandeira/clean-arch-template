namespace CleanArch.Application.Features.Samples.DTOs;

public sealed record CreateSampleRequest(
    string Name,
    string Description
);
