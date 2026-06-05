using CleanArch.Application.Features.Samples.Handlers.Update;
using CleanArch.Application.Features.Samples.Handlers.Update.Request;
using CleanArch.Application.Features.Samples.Handlers.Update.Validator;
using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Domain.MessageResource;
using CleanArch.Tests.Common.Builders;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace CleanArch.Tests.Validators.Features.Samples;

public sealed class UpdateSampleValidatorTests
{
    private readonly Mock<IGetByIdRepository<SampleEntity>> _repositoryMock = new();
    private readonly UpdateSampleValidator _sut;

    public UpdateSampleValidatorTests()
    {
        _sut = new UpdateSampleValidator(_repositoryMock.Object);
    }

    [Fact]
    public async Task ShouldBeValid_WhenCommandIsCorrect()
    {
        UpdateSampleCommand command = SampleBuilder.BuildUpdateCommand();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SampleBuilder.Build());

        ValidationResult result = await _sut.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenSampleDoesNotExist()
    {
        UpdateSampleCommand command = SampleBuilder.BuildUpdateCommand();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SampleEntity?)null);

        ValidationResult result = await _sut.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ErrorCode == SampleErrorCodes.NotFound &&
            e.ErrorMessage == ValidationMessageResource.SAMPLE_NOT_FOUND);
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenNameIsEmpty()
    {
        UpdateSampleCommand command = SampleBuilder.BuildUpdateCommand() with { Dto = new(string.Empty, "description") };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SampleBuilder.Build());

        ValidationResult result = await _sut.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(UpdateSampleRequest.Name) &&
            e.ErrorMessage == ValidationMessageResource.NAME_REQUIRED);
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenDescriptionIsEmpty()
    {
        UpdateSampleCommand command = SampleBuilder.BuildUpdateCommand() with { Dto = new("name", string.Empty) };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SampleBuilder.Build());

        ValidationResult result = await _sut.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(UpdateSampleRequest.Description) &&
            e.ErrorMessage == ValidationMessageResource.DESCRIPTION_REQUIRED);
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenNameExceedsMaxLength()
    {
        UpdateSampleCommand command = SampleBuilder.BuildUpdateCommand() with
        {
            Dto = new(new string('x', ValidationConstants.SampleRules.NameMaxLength + 1), "description")
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SampleBuilder.Build());

        ValidationResult result = await _sut.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(UpdateSampleRequest.Name) &&
            e.ErrorMessage == ValidationMessageResource.NAME_MAX_LENGTH
                .Replace("{MaxLength}", ValidationConstants.SampleRules.NameMaxLength.ToString()));
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenDescriptionExceedsMaxLength()
    {
        UpdateSampleCommand command = SampleBuilder.BuildUpdateCommand() with
        {
            Dto = new("name", new string('x', ValidationConstants.SampleRules.DescriptionMaxLength + 1))
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SampleBuilder.Build());

        ValidationResult result = await _sut.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(UpdateSampleRequest.Description) &&
            e.ErrorMessage == ValidationMessageResource.DESCRIPTION_MAX_LENGTH
                .Replace("{MaxLength}", ValidationConstants.SampleRules.DescriptionMaxLength.ToString()));
    }
}
