using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Persistence.EfCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Sample> Samples => Set<Sample>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DispatchDomainEvents();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void DispatchDomainEvents()
    {
        var aggregates = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            // Aqui você pode publicar via IPublisher (ex: MassTransit, ou seu próprio dispatcher)
            aggregate.ClearDomainEvents();
        }
    }
}
