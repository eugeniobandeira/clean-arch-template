using CleanArch.Application.Features.Samples.Handlers.Create.Request;
using CleanArch.Application.Features.Samples.Handlers.Create.Validator;
using CleanArch.Domain.Constants;
using CleanArch.Domain.MessageResource;
using CleanArch.Tests.Common.Builders;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace CleanArch.Tests.Validators.Features.Samples;

public sealed class CreateSampleValidatorTests
{
    private readonly CreateSampleValidator _sut = new();

    [Fact]
    public async Task ShouldBeValid_WhenRequestIsCorrect()
    {
        CreateSampleRequest request = SampleBuilder.BuildRequest();

        ValidationResult result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenNameIsEmpty()
    {
        CreateSampleRequest request = SampleBuilder.BuildRequest() with { Name = string.Empty };

        ValidationResult result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(request.Name) &&
            e.ErrorMessage == ValidationMessageResource.NAME_REQUIRED);
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenDescriptionIsEmpty()
    {
        CreateSampleRequest request = SampleBuilder.BuildRequest() with { Description = string.Empty };

        ValidationResult result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(request.Description) &&
            e.ErrorMessage == ValidationMessageResource.DESCRIPTION_REQUIRED);
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenNameExceedsMaxLength()
    {
        CreateSampleRequest request = SampleBuilder.BuildRequest() with
        {
            Name = new string('x', ValidationConstants.SampleRules.NameMaxLength + 1)
        };

        ValidationResult result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(request.Name) &&
            e.ErrorMessage == ValidationMessageResource.NAME_MAX_LENGTH
                .Replace("{MaxLength}", ValidationConstants.SampleRules.NameMaxLength.ToString()));
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenDescriptionExceedsMaxLength()
    {
        CreateSampleRequest request = SampleBuilder.BuildRequest() with
        {
            Description = new string('x', ValidationConstants.SampleRules.DescriptionMaxLength + 1)
        };

        ValidationResult result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(request.Description) &&
            e.ErrorMessage == ValidationMessageResource.DESCRIPTION_MAX_LENGTH
                .Replace("{MaxLength}", ValidationConstants.SampleRules.DescriptionMaxLength.ToString()));
    }
}
