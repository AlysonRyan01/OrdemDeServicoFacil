using OrdemDeServicoFacil.Domain.Shared.Models;
using OrdemDeServicoFacil.Domain.Shared.ValueObjects;

namespace OrdemDeServicoFacil.Domain.Authentication.Users;

public class User : AggregateRoot
{
    public Email Email { get; private set; } = null!;
}