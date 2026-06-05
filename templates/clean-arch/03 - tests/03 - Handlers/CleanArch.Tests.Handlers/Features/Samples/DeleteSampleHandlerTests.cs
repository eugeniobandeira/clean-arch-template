using CleanArch.Application.Features.Samples.Handlers.Delete;
using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Domain.Interfaces.UnitOfWork;
using CleanArch.Tests.Common.Builders;
using ErrorOr;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArch.Tests.Handlers.Features.Samples;

public sealed class DeleteSampleHandlerTests
{
    private readonly Mock<IGetByIdRepository<SampleEntity>> _getByIdMock = new();
    private readonly Mock<IDeleteRepository<SampleEntity>> _deleteMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = UnitOfWorkBuilder.BuildMock();
    private readonly Mock<ILogger<DeleteSampleHandler>> _loggerMock = new();
    private readonly IDeleteSampleHandler _sut;

    public DeleteSampleHandlerTests()
    {
        _deleteMock
            .Setup(r => r.DeleteAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new DeleteSampleHandler(
            _getByIdMock.Object,
            _deleteMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDeleted_WhenSampleExists()
    {
        SampleEntity sample = SampleBuilder.Build();
        _getByIdMock
            .Setup(r => r.GetByIdAsync(sample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sample);

        var result = await _sut.Handle(sample.Id);

        result.IsError.Should().BeFalse();

        _deleteMock.Verify(r => r.DeleteAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSampleDoesNotExist()
    {
        Guid id = Guid.NewGuid();
        _getByIdMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SampleEntity?)null);

        ErrorOr<Deleted> result = await _sut.Handle(id);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
        result.FirstError.Code.Should().Be(SampleErrorCodes.NotFound);

        _deleteMock.Verify(r => r.DeleteAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
