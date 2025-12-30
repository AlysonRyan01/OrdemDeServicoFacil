using OrdemDeServicoFacil.Domain.Shared.Entities;
using OrdemDeServicoFacil.Domain.Shared.Events;

namespace OrdemDeServicoFacil.Domain.Shared.Aggregates;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected AggregateRoot() { }
    
    protected AggregateRoot(Guid id) : base(id) { }
    
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}