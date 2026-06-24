using CleanArch.Domain.Entities;
using CleanArch.Infrastructure.Context.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ExampleEntity> Examples => Set<ExampleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExampleConfiguration());
    }
}
