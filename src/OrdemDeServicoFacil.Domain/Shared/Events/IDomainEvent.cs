using MediatR;

namespace OrdemDeServicoFacil.Domain.Shared.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}