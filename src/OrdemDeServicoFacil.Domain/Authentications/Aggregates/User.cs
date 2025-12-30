using OrdemDeServicoFacil.Domain.Authentication.ValueObjects;
using OrdemDeServicoFacil.Domain.Shared.Aggregates;

namespace OrdemDeServicoFacil.Domain.Authentication.Aggregates;

public class User : AggregateRoot
{
    public Email Email { get; private set; } = null!;
}