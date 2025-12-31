using OrdemDeServicoFacil.Domain.Shared.Events;

namespace OrdemDeServicoFacil.Domain.Authentication.Users.Events;

public record UserEmailUpdatedEvent(Guid UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } =  Guid.NewGuid();
}