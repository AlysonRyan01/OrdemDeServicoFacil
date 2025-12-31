using OrdemDeServicoFacil.Domain.Authentication.Users.Events;
using OrdemDeServicoFacil.Domain.Authentication.Users.ValueObjects;
using OrdemDeServicoFacil.Domain.Shared;
using OrdemDeServicoFacil.Domain.Shared.Models;
using OrdemDeServicoFacil.Domain.Shared.ValueObjects;

namespace OrdemDeServicoFacil.Domain.Authentication.Users;

public class User : AggregateRoot
{
    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public Phone Phone { get; private set; } = null!;
    public string VerificationToken { get; private set; } = null!;
    
    private User() { }
    
    private User(Email email, Password password, Phone phone)
    {
        Email = email;
        Password = password;
        Phone = phone;
        VerificationToken = GenerateNewToken();
    }

    public static Result<User> Create(Email email, Password password, Phone phone)
    {
        return Result<User>.Success(new User(email, password, phone));
    }
    
    public Result UpdatePhone(Phone phone, string verificationToken)
    {
        if (!string.Equals(verificationToken, VerificationToken, StringComparison.OrdinalIgnoreCase))
            return Result.Fail("Token de verificação inválido");
        
        if (Phone.Number == phone.Number)
            return Result.Fail("O novo número de telefone precisa ser diferente do número atual");
            
        Phone = phone;
        
        AddDomainEvent(new UserPhoneUpdatedEvent(UserId: Id, UserPhone: Phone.Number));

        VerificationToken = GenerateNewToken();
        
        return Result.Success();
    }

    public Result UpdateEmail(Email email, string verificationToken)
    {
        if (!string.Equals(verificationToken, VerificationToken, StringComparison.OrdinalIgnoreCase))
            return Result.Fail("Token de verificação inválido");
        
        if (email.Address == Email.Address)
            return Result.Fail("O novo e-mail precisa ser diferente do e-mail atual");
        
        Email =  email;
        
        AddDomainEvent(new UserEmailUpdatedEvent(UserId: Id, Email: email.Address));
        
        VerificationToken = GenerateNewToken();
        
        return Result.Success();
    }

    public Result UpdatePassword(
        string currentPasswordPlainText,
        string newPasswordPlainText)
    {
        if (!Password.Verify(currentPasswordPlainText))
            return Result.Fail("Senha atual incorreta");
    
        if (currentPasswordPlainText == newPasswordPlainText)
            return Result.Fail("A nova senha deve ser diferente da atual");
    
        var newPasswordResult = Password.Create(newPasswordPlainText);
        if (newPasswordResult.IsFailure)
            return Result.Fail(newPasswordResult.Error!);
        
        Password = newPasswordResult.Value;
    
        AddDomainEvent(new UserPasswordUpdatedEvent(UserId: Id, Email: Email.Address));
    
        return Result.Success();
    }
    
    public Result ResetPassword(string verificationToken, string newPasswordPlainText)
    {
        if (!string.Equals(verificationToken, VerificationToken, StringComparison.OrdinalIgnoreCase))
            return Result.Fail("Token de verificação inválido");
        
        var newPasswordResult = Password.Create(newPasswordPlainText);
        if (newPasswordResult.IsFailure)
            return Result.Fail(newPasswordResult.Error!);
        
        Password = newPasswordResult.Value;
        
        VerificationToken = GenerateNewToken();
        
        AddDomainEvent(new UserPasswordResetedEvent(UserId: Id, Email: Email.Address));
        
        return Result.Success();
    }
    
    private string GenerateNewToken()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}