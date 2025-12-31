using OrdemDeServicoFacil.Domain.Authentication.Users;
using OrdemDeServicoFacil.Domain.Authentication.Users.ValueObjects;
using OrdemDeServicoFacil.Domain.Shared.ValueObjects;
using System.Reflection;
using OrdemDeServicoFacil.Domain.Authentication.Users.Events;

namespace OrdemDeServicoFacil.Tests.Domain.Authentication.Users;

[TestClass]
public class UserTests
{
    private Email CreateEmail(string address) => Email.Create(address).Value;
    private Password CreatePassword(string password) => Password.Create(password).Value;
    private Phone CreatePhone(string number) => Phone.Create(number).Value;
    
    private User CreateTestUser()
    {
        var email = CreateEmail("test@example.com");
        var password = CreatePassword("Senha123!");
        var phone = CreatePhone("11999999999");
        
        return User.Create(email, password, phone).Value;
    }
    
    [TestMethod]
    public void Create_ValidUser_ReturnsSuccessResult()
    {
        // Arrange
        var email = CreateEmail("usuario@dominio.com");
        var password = CreatePassword("Senha123!");
        var phone = CreatePhone("11987654321");
        
        // Act
        var result = User.Create(email, password, phone);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(email.Address, result.Value.Email.Address);
        Assert.AreEqual(phone.Number, result.Value.Phone.Number);
        Assert.IsFalse(string.IsNullOrEmpty(result.Value.VerificationToken));
        Assert.AreEqual(6, result.Value.VerificationToken.Length);
    }
    
    [TestMethod]
    public void Create_UserHasValidVerificationToken()
    {
        // Arrange
        var user = CreateTestUser();
        
        // Act & Assert
        Assert.IsFalse(string.IsNullOrEmpty(user.VerificationToken));
        Assert.AreEqual(6, user.VerificationToken.Length);
        Assert.IsTrue(int.TryParse(user.VerificationToken, out var tokenNumber));
        Assert.IsTrue(tokenNumber >= 100000 && tokenNumber <= 999999);
    }
    
    [TestMethod]
    public void UpdatePhone_WithValidToken_Success()
    {
        // Arrange
        var user = CreateTestUser();
        var currentToken = user.VerificationToken;
        var newPhone = CreatePhone("11988887777");
        
        // Act
        var result = user.UpdatePhone(newPhone, currentToken);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(newPhone.Number, user.Phone.Number);
        Assert.AreNotEqual(currentToken, user.VerificationToken); // Token deve mudar
        Assert.HasCount(1, user.DomainEvents);
    }
    
    [TestMethod]
    public void UpdatePhone_WithInvalidToken_Fails()
    {
        // Arrange
        var user = CreateTestUser();
        var invalidToken = "000000"; // Token inválido
        var newPhone = CreatePhone("11988887777");
        
        // Act
        var result = user.UpdatePhone(newPhone, invalidToken);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Token de verificação inválido", result.Error);
        Assert.AreNotEqual(newPhone.Number, user.Phone.Number); // Não deve mudar
    }
    
    [TestMethod]
    public void UpdatePhone_WithSamePhoneNumber_Fails()
    {
        // Arrange
        var user = CreateTestUser();
        var currentToken = user.VerificationToken;
        var samePhone = CreatePhone(user.Phone.Number); // Mesmo número
        
        // Act
        var result = user.UpdatePhone(samePhone, currentToken);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("O novo número de telefone precisa ser diferente do número atual", result.Error);
    }
    
    [TestMethod]
    public void UpdatePhone_GeneratesNewVerificationToken()
    {
        // Arrange
        var user = CreateTestUser();
        var oldToken = user.VerificationToken;
        var newPhone = CreatePhone("11988887777");
        
        // Act
        var result = user.UpdatePhone(newPhone, oldToken);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreNotEqual(oldToken, user.VerificationToken);
        Assert.AreEqual(6, user.VerificationToken.Length);
    }
    
    [TestMethod]
    public void UpdateEmail_WithValidToken_Success()
    {
        // Arrange
        var user = CreateTestUser();
        var currentToken = user.VerificationToken;
        var newEmail = CreateEmail("novo@example.com");
        
        // Act
        var result = user.UpdateEmail(newEmail, currentToken);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(newEmail.Address, user.Email.Address);
        Assert.AreNotEqual(currentToken, user.VerificationToken); // Token deve mudar
        Assert.HasCount(1, user.DomainEvents);
    }
    
    [TestMethod]
    public void UpdateEmail_WithInvalidToken_Fails()
    {
        // Arrange
        var user = CreateTestUser();
        var invalidToken = "000000";
        var newEmail = CreateEmail("novo@example.com");
        
        // Act
        var result = user.UpdateEmail(newEmail, invalidToken);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Token de verificação inválido", result.Error);
        Assert.AreNotEqual(newEmail.Address, user.Email.Address); // Não deve mudar
    }
    
    [TestMethod]
    public void UpdateEmail_WithSameEmail_Fails()
    {
        // Arrange
        var user = CreateTestUser();
        var currentToken = user.VerificationToken;
        var sameEmail = CreateEmail(user.Email.Address); // Mesmo email
        
        // Act
        var result = user.UpdateEmail(sameEmail, currentToken);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("O novo e-mail precisa ser diferente do e-mail atual", result.Error);
    }
    
    [TestMethod]
    public void UpdateEmail_GeneratesNewVerificationToken()
    {
        // Arrange
        var user = CreateTestUser();
        var oldToken = user.VerificationToken;
        var newEmail = CreateEmail("novo@example.com");
        
        // Act
        var result = user.UpdateEmail(newEmail, oldToken);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreNotEqual(oldToken, user.VerificationToken);
        Assert.AreEqual(6, user.VerificationToken.Length);
    }
    
    [TestMethod]
    public void UpdatePassword_WithValidCurrentPassword_Success()
    {
        // Arrange
        var user = CreateTestUser();
        var currentPassword = "Senha123!"; // Senha original
        var newPassword = "NovaSenha456@";
        
        // Act
        var result = user.UpdatePassword(currentPassword, newPassword);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(user.Password.Verify(newPassword)); // Nova senha funciona
        Assert.IsFalse(user.Password.Verify(currentPassword)); // Senha antiga não funciona
        Assert.HasCount(1, user.DomainEvents);
    }
    
    [TestMethod]
    public void UpdatePassword_WithInvalidCurrentPassword_Fails()
    {
        // Arrange
        var user = CreateTestUser();
        var wrongPassword = "SenhaErrada!";
        var newPassword = "NovaSenha456@";
        
        // Act
        var result = user.UpdatePassword(wrongPassword, newPassword);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Senha atual incorreta", result.Error);
    }
    
    [TestMethod]
    public void UpdatePassword_WithSamePassword_Fails()
    {
        // Arrange
        var user = CreateTestUser();
        var currentPassword = "Senha123!";
        var samePassword = "Senha123!"; // Mesma senha
        
        // Act
        var result = user.UpdatePassword(currentPassword, samePassword);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("A nova senha deve ser diferente da atual", result.Error);
    }
    
    [TestMethod]
    public void UpdatePassword_WithWeakNewPassword_Fails()
    {
        // Arrange
        var user = CreateTestUser();
        var currentPassword = "Senha123!";
        var weakPassword = "123"; // Senha fraca
        
        // Act
        var result = user.UpdatePassword(currentPassword, weakPassword);
        
        // Assert
        // Password.Create vai falhar com senha fraca
        Assert.IsTrue(result.IsFailure);
        // Mensagem de erro específica do Password.Create
    }
    
    [TestMethod]
    public void UpdatePassword_DoesNotChangeVerificationToken()
    {
        // Arrange
        var user = CreateTestUser();
        var oldToken = user.VerificationToken;
        var currentPassword = "Senha123!";
        var newPassword = "NovaSenha456@";
        
        // Act
        var result = user.UpdatePassword(currentPassword, newPassword);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        // ⚠️ ATENÇÃO: Seu código NÃO muda o token no UpdatePassword!
        // Se quiser que mude, precisa adicionar: VerificationToken = GenerateNewToken();
        Assert.AreEqual(oldToken, user.VerificationToken); // Token permanece igual
    }
    
    [TestMethod]
    public void VerificationToken_AlwaysSixDigits()
    {
        // Arrange
        var user = CreateTestUser();
        
        // Act & Assert - Token inicial
        Assert.AreEqual(6, user.VerificationToken.Length);
        Assert.IsTrue(int.TryParse(user.VerificationToken, out _));
        
        // Depois de atualizar telefone
        var newPhone = CreatePhone("11988887777");
        user.UpdatePhone(newPhone, user.VerificationToken);
        Assert.AreEqual(6, user.VerificationToken.Length);
        
        // Depois de atualizar email
        var newEmail = CreateEmail("novo@example.com");
        user.UpdateEmail(newEmail, user.VerificationToken);
        Assert.AreEqual(6, user.VerificationToken.Length);
    }
    
    [TestMethod]
    public void UpdatePhone_EventContainsCorrectData()
    {
        // Arrange
        var user = CreateTestUser();
        var currentToken = user.VerificationToken;
        var newPhone = CreatePhone("11988887777");
        
        // Act
        user.UpdatePhone(newPhone, currentToken);
        
        // Assert
        var domainEvent = user.DomainEvents.First() as UserPhoneUpdatedEvent;
        Assert.IsNotNull(domainEvent);
        Assert.AreEqual(user.Id, domainEvent.UserId);
        Assert.AreEqual(newPhone.Number, domainEvent.UserPhone);
    }
    
    [TestMethod]
    public void UpdateEmail_EventContainsCorrectData()
    {
        // Arrange
        var user = CreateTestUser();
        var currentToken = user.VerificationToken;
        var newEmail = CreateEmail("novo@example.com");
        
        // Act
        user.UpdateEmail(newEmail, currentToken);
        
        // Assert
        var domainEvent = user.DomainEvents.First() as UserEmailUpdatedEvent;
        Assert.IsNotNull(domainEvent);
        Assert.AreEqual(user.Id, domainEvent.UserId);
        Assert.AreEqual(newEmail.Address, domainEvent.Email);
    }
    
    [TestMethod]
    public void UpdatePassword_EventContainsCorrectData()
    {
        // Arrange
        var user = CreateTestUser();
        var currentPassword = "Senha123!";
        var newPassword = "NovaSenha456@";
        
        // Act
        user.UpdatePassword(currentPassword, newPassword);
        
        // Assert
        var domainEvent = user.DomainEvents.First() as UserPasswordUpdatedEvent;
        Assert.IsNotNull(domainEvent);
        Assert.AreEqual(user.Id, domainEvent.UserId);
        Assert.AreEqual(user.Email.Address, domainEvent.Email);
    }
    
    [TestMethod]
    public void MultipleOperations_UpdateTokensCorrectly()
    {
        // Arrange
        var user = CreateTestUser();
        var token1 = user.VerificationToken;
        
        // Act 1: Update Phone
        var newPhone = CreatePhone("11988887777");
        var result1 = user.UpdatePhone(newPhone, token1);
        var token2 = user.VerificationToken;
        
        // Act 2: Update Email (deve usar token2)
        var newEmail = CreateEmail("novo@example.com");
        var result2 = user.UpdateEmail(newEmail, token2);
        var token3 = user.VerificationToken;
        
        // Assert
        Assert.IsTrue(result1.IsSuccess);
        Assert.IsTrue(result2.IsSuccess);
        Assert.AreNotEqual(token1, token2);
        Assert.AreNotEqual(token2, token3);
        Assert.AreNotEqual(token1, token3);
    }
    
    [TestMethod]
    public void UpdatePhone_WithExpiredToken_Fails()
    {
        // Arrange
        var user = CreateTestUser();
        var validToken = user.VerificationToken;
        
        // Usuário faz outra operação primeiro (muda token)
        var tempPhone = CreatePhone("11977776666");
        user.UpdatePhone(tempPhone, validToken);
        
        // Agora tenta usar o token antigo (expirou)
        var anotherPhone = CreatePhone("11966665555");
        var result = user.UpdatePhone(anotherPhone, validToken); // Token antigo
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Token de verificação inválido", result.Error);
    }
    
    [TestMethod]
    public void ConsecutiveUpdates_RequireNewTokens()
    {
        // Arrange
        var user = CreateTestUser();
        
        // Primeira atualização
        var token1 = user.VerificationToken;
        var phone1 = CreatePhone("11988887777");
        user.UpdatePhone(phone1, token1);
        
        // Segunda atualização precisa do NOVO token
        var token2 = user.VerificationToken;
        var phone2 = CreatePhone("11977776666");
        var result = user.UpdatePhone(phone2, token2);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    
    [TestMethod]
    public void GenerateNewToken_AlwaysReturnsValidToken()
    {
        // Testa diretamente o método privado via reflection
        var user = CreateTestUser();
        var method = typeof(User).GetMethod("GenerateNewToken", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        Assert.IsNotNull(method);
        
        // Chama várias vezes
        for (int i = 0; i < 100; i++)
        {
            var token = (string)method.Invoke(user, null)!;
            Assert.IsFalse(string.IsNullOrEmpty(token));
            Assert.AreEqual(6, token.Length);
            Assert.IsTrue(int.TryParse(token, out var tokenNumber));
            Assert.IsTrue(tokenNumber >= 100000 && tokenNumber <= 999999);
        }
    }
}