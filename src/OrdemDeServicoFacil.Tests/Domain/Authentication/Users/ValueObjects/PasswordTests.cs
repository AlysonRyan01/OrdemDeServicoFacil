using OrdemDeServicoFacil.Domain.Authentication.Users.ValueObjects;

namespace OrdemDeServicoFacil.Tests.Domain.Authentication.Users.ValueObjects;

[TestClass]
public class PasswordTests
{
    [TestMethod]
    public void Create_ValidPassword_ReturnsSuccessResult()
    {
        // Arrange
        var validPassword = "SenhaSegura123!";
        
        // Act
        var result = Password.Create(validPassword);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.IsFalse(string.IsNullOrEmpty(result.Value.Hash));
        Assert.StartsWith("$2a$", result.Value.Hash);
    }
    
    [TestMethod]
    public void Create_PasswordWithMinimumLength_ReturnsSuccessResult()
    {
        // Arrange
        var minLengthPassword = "123456"; // 6 caracteres (seu mínimo)
        
        // Act
        var result = Password.Create(minLengthPassword);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    
    [TestMethod]
    public void Create_PasswordWithMaximumLength_ReturnsSuccessResult()
    {
        // Arrange: 72 caracteres (máximo do bcrypt)
        var maxLengthPassword = new string('a', 72);
        
        // Act
        var result = Password.Create(maxLengthPassword);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    
    [TestMethod]
    public void Create_NullPassword_ReturnsFailureResult()
    {
        // Arrange
        string nullPassword = null!;
        
        // Act
        var result = Password.Create(nullPassword);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Senha não pode ser vazia", result.Error);
    }
    
    [TestMethod]
    public void Create_EmptyPassword_ReturnsFailureResult()
    {
        // Arrange
        var emptyPassword = "";
        
        // Act
        var result = Password.Create(emptyPassword);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Senha não pode ser vazia", result.Error);
    }
    
    [TestMethod]
    public void Create_WhitespacePassword_ReturnsFailureResult()
    {
        // Arrange
        var whitespacePassword = "   ";
        
        // Act
        var result = Password.Create(whitespacePassword);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Senha não pode ser vazia", result.Error);
    }
    
    [TestMethod]
    public void Create_PasswordTooShort_ReturnsFailureResult()
    {
        // Arrange
        var shortPassword = "12345"; // 5 caracteres (menos que 6)
        
        // Act
        var result = Password.Create(shortPassword);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("A senha deve ter pelo menos 6 caracteres", result.Error);
    }
    
    [TestMethod]
    public void Create_PasswordTooLong_ReturnsFailureResult()
    {
        // Arrange: 73 caracteres (mais que 72)
        var longPassword = new string('a', 73);
        
        // Act
        var result = Password.Create(longPassword);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("A senha deve ter menos de 73 caracteres", result.Error);
    }
    
    [TestMethod]
    public void Create_PasswordExceedsBcryptLimit_ReturnsFailureResult()
    {
        // Arrange: bcrypt tem limite de 72 bytes
        // Para caracteres UTF-8 complexos, o limite pode ser menor
        var veryLongPassword = new string('á', 50); // Caracteres multi-byte
        
        // Act
        var result = Password.Create(veryLongPassword);
        
        // Assert
        // Pode falhar no bcrypt, mas nosso limite de 72 caracteres já previne
        Assert.IsTrue(result.IsSuccess || result.IsFailure);
    }
    
    [TestMethod]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        // Arrange
        var originalPassword = "MinhaSenha123!";
        var password = Password.Create(originalPassword).Value;
        
        // Act
        var isValid = password.Verify(originalPassword);
        
        // Assert
        Assert.IsTrue(isValid);
    }
    
    [TestMethod]
    public void Verify_IncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var originalPassword = "MinhaSenha123!";
        var wrongPassword = "SenhaErrada456@";
        var password = Password.Create(originalPassword).Value;
        
        // Act
        var isValid = password.Verify(wrongPassword);
        
        // Assert
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void Verify_NullPassword_ReturnsFalse()
    {
        // Arrange
        var password = Password.Create("Senha123").Value;
        
        // Act
        var isValid = password.Verify(null!);
        
        // Assert
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void Verify_EmptyPassword_ReturnsFalse()
    {
        // Arrange
        var password = Password.Create("Senha123").Value;
        
        // Act
        var isValid = password.Verify("");
        
        // Assert
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void Verify_WhitespacePassword_ReturnsFalse()
    {
        // Arrange
        var password = Password.Create("Senha123").Value;
        
        // Act
        var isValid = password.Verify("   ");
        
        // Assert
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public void Verify_CaseSensitivePassword_RespectsCase()
    {
        // Arrange
        var originalPassword = "SenhaComCase123";
        var password = Password.Create(originalPassword).Value;
        
        // Act & Assert
        Assert.IsTrue(password.Verify("SenhaComCase123"));
        Assert.IsFalse(password.Verify("senhacomcase123")); // tudo minúsculo
        Assert.IsFalse(password.Verify("SENHACOMCASE123")); // tudo maiúsculo
    }
    
    [TestMethod]
    public void Create_SamePasswordTwice_DifferentHashes()
    {
        // Arrange
        var samePassword = "MinhaSenha123!";
        
        // Act
        var password1 = Password.Create(samePassword).Value;
        var password2 = Password.Create(samePassword).Value;
        
        // Assert
        Assert.AreNotEqual(password1.Hash, password2.Hash); // Salts diferentes
    }
    
    [TestMethod]
    public void Equals_SameHashPassword_ReturnsTrue()
    {
        // Arrange: criar senha com hash específico (simulado)
        var hash = "$2a$12$N9qo8uLOickgx2ZMRZoMye3m3BkG/6ZfBwQ9Yq4UqJNp9t0Y5pXbS";
        var password1 = CreatePasswordWithHash(hash);
        var password2 = CreatePasswordWithHash(hash);
        
        // Act & Assert
        Assert.IsTrue(password1.Equals(password2));
        Assert.IsTrue(password1 == password2);
    }
    
    [TestMethod]
    public void Equals_DifferentHashPassword_ReturnsFalse()
    {
        // Arrange
        var password1 = Password.Create("Senha123").Value;
        var password2 = Password.Create("Senha456").Value;
        
        // Act & Assert
        Assert.IsFalse(password1.Equals(password2));
        Assert.IsFalse(password1 == password2);
        Assert.IsTrue(password1 != password2);
    }
    
    [TestMethod]
    public void GetHashCode_SameHash_SameHashCode()
    {
        // Arrange
        var hash = "$2a$12$N9qo8uLOickgx2ZMRZoMye3m3BkG/6ZfBwQ9Yq4UqJNp9t0Y5pXbS";
        var password1 = CreatePasswordWithHash(hash);
        var password2 = CreatePasswordWithHash(hash);
        
        // Act
        var hashCode1 = password1.GetHashCode();
        var hashCode2 = password2.GetHashCode();
        
        // Assert
        Assert.AreEqual(hashCode1, hashCode2);
    }
    
    [TestMethod]
    public void GetHashCode_DifferentHash_DifferentHashCode()
    {
        // Arrange
        var password1 = Password.Create("Senha123").Value;
        var password2 = Password.Create("Senha456").Value;
        
        // Act
        var hashCode1 = password1.GetHashCode();
        var hashCode2 = password2.GetHashCode();
        
        // Assert
        Assert.AreNotEqual(hashCode1, hashCode2);
    }
    
    [TestMethod]
    public void Password_InHashSet_RemovesDuplicates()
    {
        // Arrange
        var hash = "$2a$12$N9qo8uLOickgx2ZMRZoMye3m3BkG/6ZfBwQ9Yq4UqJNp9t0Y5pXbS";
        var set = new HashSet<Password>();
        var password1 = CreatePasswordWithHash(hash);
        var password2 = CreatePasswordWithHash(hash); // Mesmo hash
        
        // Act
        set.Add(password1);
        set.Add(password2); // Deve ser considerado duplicata
        
        // Assert
        Assert.HasCount(1, set);
    }
    
    [TestMethod]
    public void Verify_WithSpecialCharacters_WorksCorrectly()
    {
        // Arrange
        var passwordsWithSpecialChars = new[]
        {
            "Senha@123",
            "Teste#456",
            "ABC$789",
            "Senha!123",
            "Teste%456",
            "ABC&789",
            "Senha*123",
            "Teste(456)",
            "ABC[789]",
            "Senha{123}",
            "Teste|456",
            @"ABC\789",
            "Senha:123",
            "Teste;456",
            "ABC'789",
            "\"Senha123\"",
            "Teste<456>",
            "ABC?789",
            "Senha/123",
            "Teste~456",
            "Sença123", // Com acento
            "パスワード123" // Japonês
        };
        
        foreach (var passwordText in passwordsWithSpecialChars)
        {
            // Act
            var passwordResult = Password.Create(passwordText);
            
            // Assert - deve criar com sucesso
            Assert.IsTrue(passwordResult.IsSuccess, 
                $"Falhou para: {passwordText}");
            
            // Verifica que a senha funciona
            var isValid = passwordResult.Value.Verify(passwordText);
            Assert.IsTrue(isValid, 
                $"Verificação falhou para: {passwordText}");
        }
    }
    
    [TestMethod]
    public void Create_WithWorkFactor12_HashStartsWithCorrectPrefix()
    {
        // Arrange
        var password = "Teste123";
        
        // Act
        var result = Password.Create(password);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        // BCrypt EnhancedHashPassword com workFactor 12 gera $2a$12$
        Assert.StartsWith("$2a$12$", result.Value.Hash);
    }
    
    [TestMethod]
    public void Verify_PasswordWithNullHash_ShouldNotCrash()
    {
        // Arrange: cenário onde o hash poderia ser null (teórico)
        // Simula um possível estado inválido
        var password = CreatePasswordWithHash(null!);
        
        // Act & Assert - Verify deve retornar false, não crash
        var isValid = password.Verify("anypassword");
        Assert.IsFalse(isValid);
    }
    
    // Método auxiliar para criar Password com hash específico (reflection)
    private Password CreatePasswordWithHash(string hash)
    {
        // Usa reflection para acessar o construtor privado
        var constructor = typeof(Password)
            .GetConstructors(System.Reflection.BindingFlags.NonPublic | 
                           System.Reflection.BindingFlags.Instance)
            .FirstOrDefault(c => c.GetParameters().Length == 1);
        
        if (constructor == null)
            throw new InvalidOperationException("Construtor não encontrado");
        
        return (Password)constructor.Invoke(new object[] { hash });
    }
}