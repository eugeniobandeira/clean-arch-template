using CleanArch.Application.Features.Samples.Handlers.GetById;
using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Tests.Common.Builders;
using ErrorOr;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArch.Tests.Handlers.Features.Samples;

public sealed class GetByIdSampleHandlerTests
{
    private readonly Mock<IGetByIdRepository<SampleEntity>> _repositoryMock = new();
    private readonly Mock<ILogger<GetByIdSampleHandler>> _loggerMock = new();
    private readonly IGetByIdSampleHandler _sut;

    public GetByIdSampleHandlerTests()
    {
        _sut = new GetByIdSampleHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSample_WhenFound()
    {
        SampleEntity sample = SampleBuilder.Build();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(sample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sample);

        var result = await _sut.Handle(sample.Id);

        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(sample.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenNotFound()
    {
        Guid id = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SampleEntity?)null);

        ErrorOr<SampleEntity> result = await _sut.Handle(id);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
        result.FirstError.Code.Should().Be(SampleErrorCodes.NotFound);
    }
}
