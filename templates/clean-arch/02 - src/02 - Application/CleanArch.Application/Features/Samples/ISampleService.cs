using CleanArch.Application.Features.Samples.DTOs;

namespace CleanArch.Application.Features.Samples;

public interface ISampleService
{
    Task<SampleResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SampleResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SampleResponse> CreateAsync(CreateSampleRequest request, CancellationToken cancellationToken = default);
    Task<SampleResponse> UpdateAsync(Guid id, CreateSampleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
