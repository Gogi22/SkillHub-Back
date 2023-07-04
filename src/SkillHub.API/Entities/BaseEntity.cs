namespace SkillHub.API.Entities;

public abstract class BaseEntity<T> : IAuditableEntity
{
    public T Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}