using CleanArch.Application.Features.Samples.Handlers.Update;
using CleanArch.Application.Features.Samples.Handlers.Update.Validator;
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

public sealed class UpdateSampleHandlerTests
{
    private readonly Mock<IGetByIdRepository<SampleEntity>> _getByIdMock = new();
    private readonly Mock<IUpdateRepository<SampleEntity>> _updateMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = UnitOfWorkBuilder.BuildMock();
    private readonly Mock<ILogger<UpdateSampleHandler>> _loggerMock = new();
    private readonly IUpdateSampleHandler _sut;

    public UpdateSampleHandlerTests()
    {
        _updateMock
            .Setup(r => r.UpdateAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new UpdateSampleHandler(
            _getByIdMock.Object,
            _updateMock.Object,
            new UpdateSampleValidator(_getByIdMock.Object),
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUpdatedSample_WhenCommandIsValid()
    {
        SampleEntity sample = SampleBuilder.Build();
        UpdateSampleCommand command = SampleBuilder.BuildUpdateCommand(sample.Id);

        _getByIdMock
            .Setup(r => r.GetByIdAsync(sample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sample);

        ErrorOr<SampleEntity> result = await _sut.Handle(command);

        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be(command.Dto.Name);
        result.Value.Description.Should().Be(command.Dto.Description);

        _updateMock.Verify(r => r.UpdateAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSampleDoesNotExist()
    {
        UpdateSampleCommand command = SampleBuilder.BuildUpdateCommand();

        _getByIdMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SampleEntity?)null);

        ErrorOr<SampleEntity> result = await _sut.Handle(command);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
        result.FirstError.Code.Should().Be(SampleErrorCodes.NotFound);

        _updateMock.Verify(r => r.UpdateAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationErrors_WhenRequestIsInvalid()
    {
        Guid id = Guid.NewGuid();
        UpdateSampleCommand command = new(id, new(string.Empty, string.Empty));

        _getByIdMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SampleBuilder.Build());

        ErrorOr<SampleEntity> result = await _sut.Handle(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().AllSatisfy(e => e.Type.Should().Be(ErrorType.Validation));

        _updateMock.Verify(r => r.UpdateAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
