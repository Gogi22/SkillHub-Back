using System.Collections.Immutable;

namespace IdentityServer.Entities;

public abstract class BaseAggregateRoot<TKey> : BaseEntity<TKey>, IAggregateRoot
{
    private readonly IList<IDomainEvent> _events;

    protected BaseAggregateRoot()
    {
        _events = new List<IDomainEvent>();
    }

    public IReadOnlyCollection<IDomainEvent> Events => _events.ToImmutableArray();

    public void ClearEvents()
    {
        _events.Clear();
    }

    protected void AddEvent<TE>(TE @event) where TE : IDomainEvent
    {
        _events.Add(@event);
    }
}

public abstract class BaseEntity<TKey>
{
    public TKey Id { get; }

    public override bool Equals(object? obj)
    {
        var entity = obj as BaseEntity<TKey>;
        return entity != null &&
               GetType() == entity.GetType() &&
               EqualityComparer<TKey>.Default.Equals(Id, entity.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }

    public static bool operator ==(BaseEntity<TKey>? entity1, BaseEntity<TKey>? entity2)
    {
        return EqualityComparer<BaseEntity<TKey>>.Default.Equals(entity1, entity2);
    }

    public static bool operator !=(BaseEntity<TKey>? entity1, BaseEntity<TKey>? entity2)
    {
        return !(entity1 == entity2);
    }
}