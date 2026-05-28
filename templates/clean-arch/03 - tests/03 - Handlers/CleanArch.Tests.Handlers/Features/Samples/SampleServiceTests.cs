using CleanArch.Application.Features.Samples;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Repositories;
using CleanArch.Tests.Common.Builders;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArch.Tests.Handlers.Features.Samples;

public sealed class SampleServiceTests
{
    private readonly Mock<ISampleRepository> _repositoryMock = new();
    private readonly Mock<ILogger<SampleService>> _loggerMock = new();
    private readonly ISampleService _sut;

    public SampleServiceTests()
    {
        _sut = new SampleService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnResponse_WhenRequestIsValid()
    {
        var request = SampleBuilder.BuildRequest();

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Sample>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(request);

        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        result.IsActive.Should().BeTrue();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Sample>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sample?)null);

        var result = await _sut.GetByIdAsync(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnResponse_WhenFound()
    {
        var sample = SampleBuilder.Build();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(sample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sample);

        var result = await _sut.GetByIdAsync(sample.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(sample.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sample?)null);

        var act = async () => await _sut.UpdateAsync(id, SampleBuilder.BuildRequest());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
