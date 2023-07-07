namespace SkillHub.API.Entities;

public abstract class BaseEntity<T> : IAuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public T Id { get; set; } = default!;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    protected void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}