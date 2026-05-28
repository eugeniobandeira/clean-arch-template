using CleanArch.Application.Features.Samples.Validators;
using CleanArch.Tests.Common.Builders;
using FluentAssertions;
using Xunit;

namespace CleanArch.Tests.Validators.Features.Samples;

public sealed class CreateSampleValidatorTests
{
    private readonly CreateSampleValidator _sut = new();

    [Fact]
    public async Task ShouldBeValid_WhenRequestIsCorrect()
    {
        var request = SampleBuilder.BuildRequest();

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenNameIsEmpty()
    {
        var request = SampleBuilder.BuildRequest() with { Name = string.Empty };

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(request.Name));
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenDescriptionIsEmpty()
    {
        var request = SampleBuilder.BuildRequest() with { Description = string.Empty };

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(request.Description));
    }

    [Fact]
    public async Task ShouldBeInvalid_WhenNameExceedsMaxLength()
    {
        var request = SampleBuilder.BuildRequest() with { Name = new string('x', 101) };

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(request.Name));
    }
}
