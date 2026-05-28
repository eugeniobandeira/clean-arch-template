using CleanArch.Domain.Common;

namespace CleanArch.Domain.Entities;

public sealed class Sample : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private Sample() { }

    public static Sample Create(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        var sample = new Sample
        {
            Name = name,
            Description = description
        };

        sample.RaiseDomainEvent(new SampleCreatedEvent(sample.Id, sample.Name));

        return sample;
    }

    public void Update(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Name = name;
        Description = description;
        SetUpdatedAt();
    }

    public void Deactivate() => IsActive = false;
}

public sealed record SampleCreatedEvent(Guid SampleId, string Name) : DomainEvent;
