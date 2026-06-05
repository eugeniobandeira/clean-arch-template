using CleanArch.Application.Features.Samples.Handlers.Create;
using CleanArch.Application.Features.Samples.Handlers.Create.Request;
using CleanArch.Application.Features.Samples.Handlers.Create.Validator;
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

public sealed class CreateSampleHandlerTests
{
    private readonly Mock<IAddRepository<SampleEntity>> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = UnitOfWorkBuilder.BuildMock();
    private readonly Mock<ILogger<CreateSampleHandler>> _loggerMock = new();
    private readonly ICreateSampleHandler _sut;

    public CreateSampleHandlerTests()
    {
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new CreateSampleHandler(
            _repositoryMock.Object,
            new CreateSampleValidator(),
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSample_WhenRequestIsValid()
    {
        CreateSampleRequest request = SampleBuilder.BuildRequest();

        ErrorOr<SampleEntity> result = await _sut.Handle(request);

        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);
        result.Value.IsActive.Should().BeTrue();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationErrors_WhenRequestIsInvalid()
    {
        CreateSampleRequest request = SampleBuilder.BuildInvalidRequest();

        ErrorOr<SampleEntity> result = await _sut.Handle(request);

        result.IsError.Should().BeTrue();
        result.Errors.Should().AllSatisfy(e => e.Type.Should().Be(ErrorType.Validation));

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<SampleEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
