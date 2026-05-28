using CleanArch.Application.Features.Samples.DTOs;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Samples;

public sealed class SampleService(
    ISampleRepository repository,
    ILogger<SampleService> logger) : ISampleService
{
    public async Task<SampleResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching sample {SampleId}", id);

        var sample = await repository.GetByIdAsync(id, cancellationToken);
        return sample is null ? null : MapToResponse(sample);
    }

    public async Task<IEnumerable<SampleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching all samples");

        var samples = await repository.GetAllAsync(cancellationToken);
        return samples.Select(MapToResponse);
    }

    public async Task<SampleResponse> CreateAsync(CreateSampleRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating sample {Name}", request.Name);

        var sample = Sample.Create(request.Name, request.Description);
        await repository.AddAsync(sample, cancellationToken);

        logger.LogInformation("Sample {SampleId} created successfully", sample.Id);

        return MapToResponse(sample);
    }

    public async Task<SampleResponse> UpdateAsync(Guid id, CreateSampleRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating sample {SampleId}", id);

        var sample = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sample {id} not found.");

        sample.Update(request.Name, request.Description);
        await repository.UpdateAsync(sample, cancellationToken);

        return MapToResponse(sample);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting sample {SampleId}", id);

        await repository.DeleteAsync(id, cancellationToken);
    }

    private static SampleResponse MapToResponse(Sample sample) =>
        new(sample.Id, sample.Name, sample.Description, sample.IsActive, sample.CreatedAt, sample.UpdatedAt);
}
