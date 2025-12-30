using System.Text.RegularExpressions;
using OrdemDeServicoFacil.Domain.Shared;

namespace OrdemDeServicoFacil.Domain.Authentication.ValueObjects;

public class Email : ValueObject
{
    public string Address { get; }

    private Email(string address)
    {
        Address = address;
    }
    
    public static Result<Email> Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return Result<Email>.Fail("Endereço de email não pode ser vazio");
        
        var normalizedAddress = address.Trim();
        
        var emailRegex = new Regex(
            @"^(?!\.)(?!.*\.\.)([a-zA-Z0-9._%+-]+)@([a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$",
            RegexOptions.Compiled);
        
        if (!emailRegex.IsMatch(normalizedAddress))
            return Result<Email>.Fail("Formato de email inválido");

        return Result<Email>.Success(new Email(normalizedAddress.ToLowerInvariant()));
    }
    
    public override string ToString() => Address.Trim().ToLowerInvariant();
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}