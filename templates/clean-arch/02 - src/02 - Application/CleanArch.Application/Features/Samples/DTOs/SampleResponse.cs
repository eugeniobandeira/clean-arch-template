namespace CleanArch.Application.Features.Samples.DTOs;

public sealed record SampleResponse(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
