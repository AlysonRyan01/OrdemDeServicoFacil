using OrdemDeServicoFacil.Domain.Shared.Events;

namespace OrdemDeServicoFacil.Domain.Authentication.Users.Events;

public record UserPhoneUpdatedEvent(Guid UserId, string UserPhone) : IDomainEvent
{
    public DateTime OccurredOn { get; } =  DateTime.UtcNow;
    public Guid EventId { get; } =  Guid.NewGuid();
}