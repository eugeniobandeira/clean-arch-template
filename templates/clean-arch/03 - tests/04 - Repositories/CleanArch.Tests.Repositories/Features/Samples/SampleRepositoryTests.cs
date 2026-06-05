using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
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
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _sut = new SampleEfRepository(_dbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistSample()
    {
        SampleEntity sample = SampleBuilder.Build();

        await _sut.AddAsync(sample);
        await _dbContext.SaveChangesAsync();

        var persisted = await _sut.GetByIdAsync(sample.Id);
        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be(sample.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        SampleEntity? result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActive_WhenFilterIsTrue()
    {
        SampleEntity active = SampleBuilder.Build();
        SampleEntity inactive = SampleBuilder.Build();
        inactive.Deactivate();

        await _sut.AddAsync(active);
        await _sut.AddAsync(inactive);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync(new SampleFilter(IsActive: true));

        result.Items.Should().HaveCount(1);
        result.Items.First().Id.Should().Be(active.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAll_WhenFilterIsNull()
    {
        SampleEntity active = SampleBuilder.Build();
        SampleEntity inactive = SampleBuilder.Build();
        inactive.Deactivate();

        await _sut.AddAsync(active);
        await _sut.AddAsync(inactive);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync(new SampleFilter(IsActive: null));

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifySample()
    {
        SampleEntity sample = SampleBuilder.Build();
        await _sut.AddAsync(sample);
        await _dbContext.SaveChangesAsync();

        const string updatedName = "Updated Name";
        const string updatedDescription = "Updated Description";
        sample.Update(updatedName, updatedDescription);

        await _sut.UpdateAsync(sample);
        await _dbContext.SaveChangesAsync();

        var updated = await _sut.GetByIdAsync(sample.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be(updatedName);
        updated.Description.Should().Be(updatedDescription);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSample()
    {
        SampleEntity sample = SampleBuilder.Build();
        await _sut.AddAsync(sample);
        await _dbContext.SaveChangesAsync();

        await _sut.DeleteAsync(sample);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(sample.Id);
        result.Should().BeNull();
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _dbContext.DisposeAsync();
}
