using CleanArch.Application.Features.Samples.Handlers.GetAll;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Tests.Common.Builders;
using ErrorOr;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArch.Tests.Handlers.Features.Samples;

public sealed class GetAllSampleHandlerTests
{
    private readonly Mock<IGetAllRepository<SampleEntity, SampleFilter>> _repositoryMock = new();
    private readonly Mock<ILogger<GetAllSampleHandler>> _loggerMock = new();
    private readonly IGetAllSampleHandler _sut;

    public GetAllSampleHandlerTests()
    {
        _sut = new GetAllSampleHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WhenSamplesExist()
    {
        List<SampleEntity> samples = SampleBuilder.BuildList(3);
        SampleFilter filter = new(Page: 1, PageSize: 10);
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.Is<SampleFilter>(f => f.IsActive == true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<SampleEntity>(samples, 3));

        ErrorOr<PagedResult<SampleEntity>> result = await _sut.Handle(filter);

        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(3);
        result.Value.Total.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResult_WhenNoSamplesExist()
    {
        SampleFilter filter = new(Page: 1, PageSize: 10);
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.Is<SampleFilter>(f => f.IsActive == true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<SampleEntity>([], 0));

        ErrorOr<PagedResult<SampleEntity>> result = await _sut.Handle(filter);

        result.IsError.Should().BeFalse();
        result.Value.Items.Should().BeEmpty();
        result.Value.Total.Should().Be(0);
    }
}
