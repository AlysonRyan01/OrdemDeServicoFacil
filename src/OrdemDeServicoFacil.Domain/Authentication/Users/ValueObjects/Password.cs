using OrdemDeServicoFacil.Domain.Shared;
using OrdemDeServicoFacil.Domain.Shared.Models;

namespace OrdemDeServicoFacil.Domain.Authentication.Users.ValueObjects;

public class Password : ValueObject
{
    public string Hash { get; private set; }
    
    private  Password(string hash)
    {
        Hash = hash;
    }

    public static Result<Password> Create(string textPassword)
    {
        if (string.IsNullOrWhiteSpace(textPassword))
            return Result<Password>.Fail("Senha não pode ser vazia");
        
        if (textPassword.Length < 6)
            return Result<Password>.Fail("A senha deve ter pelo menos 6 caracteres");
        
        if (textPassword.Length > 72)
            return Result<Password>.Fail("A senha deve ter menos de 73 caracteres");
        
        string hash;
        try
        {
            hash = BCrypt.Net.BCrypt.EnhancedHashPassword(textPassword, workFactor: 12);
        }
        catch
        {
            return Result<Password>.Fail("Erro ao processar a senha");
        }
        
        return Result<Password>.Success(new Password(hash));
    }

    public bool Verify(string textPasswordToVerify)
    {
        if (string.IsNullOrWhiteSpace(textPasswordToVerify))
            return false;
            
        try
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(textPasswordToVerify, Hash);
        }
        catch
        {
            return false;
        }
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
    }
}