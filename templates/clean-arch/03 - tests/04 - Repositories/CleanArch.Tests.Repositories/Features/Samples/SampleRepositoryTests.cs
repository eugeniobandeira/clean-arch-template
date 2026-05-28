using CleanArch.Infrastructure.Persistence.EfCore;
using CleanArch.Infrastructure.Repositories.EfCore;
using CleanArch.Tests.Common.Builders;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CleanArch.Tests.Repositories.Features.Samples;

public sealed class SampleRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext;
    private readonly SampleEfRepository _sut;

    public SampleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _sut = new SampleEfRepository(_dbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistSample()
    {
        var sample = SampleBuilder.Build();

        await _sut.AddAsync(sample);

        var persisted = await _sut.GetByIdAsync(sample.Id);
        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be(sample.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActive()
    {
        var active = SampleBuilder.Build();
        var inactive = SampleBuilder.Build();
        inactive.Deactivate();

        await _sut.AddAsync(active);
        await _sut.AddAsync(inactive);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(active.Id);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSample()
    {
        var sample = SampleBuilder.Build();
        await _sut.AddAsync(sample);

        await _sut.DeleteAsync(sample.Id);

        var result = await _sut.GetByIdAsync(sample.Id);
        result.Should().BeNull();
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _dbContext.DisposeAsync();
}
