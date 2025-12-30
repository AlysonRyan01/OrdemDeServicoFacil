using OrdemDeServicoFacil.Domain.Authentication.ValueObjects;

namespace OrdemDeServicoFacil.Tests.Domain.Shared.ValueObjects;

[TestClass]
public class EmailTests
{
    [TestMethod]
    public void Create_ValidEmail_ReturnsSuccessResult()
    {
        // Arrange
        var validEmail = "usuario@dominio.com";
        
        // Act
        var result = Email.Create(validEmail);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("usuario@dominio.com", result.Value.Address);
    }
    
    [TestMethod]
    public void Create_ValidEmailWithUppercase_NormalizesToLowercase()
    {
        // Arrange
        var emailWithUppercase = "USUARIO@DOMINIO.COM";
        
        // Act
        var result = Email.Create(emailWithUppercase);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("usuario@dominio.com", result.Value.Address);
    }
    
    [TestMethod]
    public void Create_ValidEmailWithSpaces_TrimsSpaces()
    {
        // Arrange
        var emailWithSpaces = "  usuario@dominio.com  ";
        
        // Act
        var result = Email.Create(emailWithSpaces);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("usuario@dominio.com", result.Value.Address);
    }
    
    [TestMethod]
    public void Create_ValidEmailWithSpecialCharacters_ReturnsSuccess()
    {
        // Arrange
        var emails = new[]
        {
            "nome.sobrenome@dominio.com",
            "nome+tag@dominio.com",
            "nome_sobrenome@dominio.com",
            "123numero@dominio.com",
            "nome-sobrenome@dominio.com"
        };
        
        foreach (var email in emails)
        {
            // Act
            var result = Email.Create(email);
            
            // Assert
            Assert.IsTrue(result.IsSuccess, $"Falhou para: {email}");
            Assert.AreEqual(email.ToLower(), result.Value.Address);
        }
    }
    
    [TestMethod]
    public void Create_NullEmail_ReturnsFailureResult()
    {
        // Arrange
        string nullEmail = null!;
        
        // Act
        var result = Email.Create(nullEmail);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Endereço de email não pode ser vazio", result.Error);
    }
    
    [TestMethod]
    public void Create_EmptyEmail_ReturnsFailureResult()
    {
        // Arrange
        var emptyEmail = "";
        
        // Act
        var result = Email.Create(emptyEmail);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Endereço de email não pode ser vazio", result.Error);
    }
    
    [TestMethod]
    public void Create_WhitespaceEmail_ReturnsFailureResult()
    {
        // Arrange
        var whitespaceEmail = "   ";
        
        // Act
        var result = Email.Create(whitespaceEmail);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Endereço de email não pode ser vazio", result.Error);
    }
    
    [TestMethod]
    public void Create_EmailWithoutAtSymbol_ReturnsFailureResult()
    {
        // Arrange
        var invalidEmail = "usuario.dominio.com";
        
        // Act
        var result = Email.Create(invalidEmail);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Formato de email inválido", result.Error);
    }
    
    [TestMethod]
    public void Create_EmailWithoutDomain_ReturnsFailureResult()
    {
        // Arrange
        var invalidEmail = "usuario@";
        
        // Act
        var result = Email.Create(invalidEmail);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Formato de email inválido", result.Error);
    }
    
    [TestMethod]
    public void Create_EmailWithoutLocalPart_ReturnsFailureResult()
    {
        // Arrange
        var invalidEmail = "@dominio.com";
        
        // Act
        var result = Email.Create(invalidEmail);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Formato de email inválido", result.Error);
    }
    
    [TestMethod]
    public void Create_EmailWithConsecutiveDots_ReturnsFailureResult()
    {
        // Arrange
        var invalidEmail = "usuario..nome@dominio.com";
        
        // Act
        var result = Email.Create(invalidEmail);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Formato de email inválido", result.Error);
    }
    
    [TestMethod]
    public void Create_EmailStartingWithDot_ReturnsFailureResult()
    {
        // Arrange
        var invalidEmail = ".usuario@dominio.com";
        
        // Act
        var result = Email.Create(invalidEmail);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Formato de email inválido", result.Error);
    }
    
    [TestMethod]
    public void Create_EmailWithSingleCharDomain_ReturnsFailureResult()
    {
        // Arrange
        var invalidEmail = "usuario@d.c";
        
        // Act
        var result = Email.Create(invalidEmail);
        
        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Formato de email inválido", result.Error);
    }
    
    [TestMethod]
    public void Create_EmailWithInvalidCharacters_ReturnsFailureResult()
    {
        // Arrange
        var invalidEmails = new[]
        {
            "usuário@dominio.com",     // acento
            "user name@dominio.com",   // espaço
            "user@domínio.com",        // acento no domínio
            "user@dominio.c",          // TLD muito curto
        };
        
        foreach (var email in invalidEmails)
        {
            // Act
            var result = Email.Create(email);
            
            // Assert
            Assert.IsTrue(result.IsFailure, $"Deveria falhar para: {email}");
            Assert.AreEqual("Formato de email inválido", result.Error, $"Mensagem errada para: {email}");
        }
    }
    
    [TestMethod]
    public void ToString_ReturnsEmailAddress()
    {
        // Arrange
        var emailAddress = "usuario@dominio.com";
        var email = Email.Create(emailAddress).Value;
        
        // Act
        var result = email.ToString();
        
        // Assert
        Assert.AreEqual(emailAddress, result);
    }
    
    [TestMethod]
    public void Equals_SameEmailAddress_ReturnsTrue()
    {
        // Arrange
        var email1 = Email.Create("usuario@dominio.com").Value;
        var email2 = Email.Create("usuario@dominio.com").Value;
        
        // Act & Assert
        Assert.IsTrue(email1.Equals(email2));
        Assert.IsTrue(email1 == email2);
        Assert.IsFalse(email1 != email2);
    }
    
    [TestMethod]
    public void Equals_DifferentEmailAddress_ReturnsFalse()
    {
        // Arrange
        var email1 = Email.Create("usuario1@dominio.com").Value;
        var email2 = Email.Create("usuario2@dominio.com").Value;
        
        // Act & Assert
        Assert.IsFalse(email1.Equals(email2));
        Assert.IsFalse(email1 == email2);
        Assert.IsTrue(email1 != email2);
    }
    
    [TestMethod]
    public void Equals_SameEmailDifferentCase_ReturnsTrue()
    {
        // Arrange
        var email1 = Email.Create("USUARIO@DOMINIO.COM").Value;
        var email2 = Email.Create("usuario@dominio.com").Value;
        
        // Act & Assert
        Assert.IsTrue(email1.Equals(email2));
        Assert.IsTrue(email1 == email2);
    }
    
    [TestMethod]
    public void GetHashCode_SameEmail_SameHashCode()
    {
        // Arrange
        var email1 = Email.Create("usuario@dominio.com").Value;
        var email2 = Email.Create("usuario@dominio.com").Value;
        
        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();
        
        // Assert
        Assert.AreEqual(hashCode1, hashCode2);
    }
    
    [TestMethod]
    public void GetHashCode_DifferentEmail_DifferentHashCode()
    {
        // Arrange
        var email1 = Email.Create("usuario1@dominio.com").Value;
        var email2 = Email.Create("usuario2@dominio.com").Value;
        
        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();
        
        // Assert
        Assert.AreNotEqual(hashCode1, hashCode2);
    }
    
    [TestMethod]
    public void Email_CanBeUsedAsDictionaryKey()
    {
        // Arrange
        var dictionary = new Dictionary<Email, string>();
        var email1 = Email.Create("usuario@dominio.com").Value;
        var email2 = Email.Create("usuario@dominio.com").Value; // Mesmo valor
        
        // Act
        dictionary[email1] = "Valor 1";
        dictionary[email2] = "Valor 2"; // Deve sobrescrever
        
        // Assert
        Assert.HasCount(1, dictionary);
        Assert.AreEqual("Valor 2", dictionary[email1]);
    }
    
    [TestMethod]
    public void Email_InHashSet_RemovesDuplicates()
    {
        // Arrange
        var hashSet = new HashSet<Email>();
        var email1 = Email.Create("usuario@dominio.com").Value;
        var email2 = Email.Create("usuario@dominio.com").Value;
        var email3 = Email.Create("outro@dominio.com").Value;
        
        // Act
        hashSet.Add(email1);
        hashSet.Add(email2); // Duplicata
        hashSet.Add(email3);
        
        // Assert
        Assert.HasCount(2, hashSet);
        Assert.Contains(email1, hashSet);
        Assert.Contains(email3, hashSet);
    }
    
    [TestMethod]
    public void Email_InListDistinct_RemovesDuplicates()
    {
        // Arrange
        var emails = new List<Email>
        {
            Email.Create("usuario@dominio.com").Value,
            Email.Create("usuario@dominio.com").Value, // Duplicata
            Email.Create("outro@dominio.com").Value
        };
        
        // Act
        var distinctEmails = emails.Distinct().ToList();
        
        // Assert
        Assert.HasCount(2, distinctEmails);
    }
    
    [TestMethod]
    public void Email_WithDifferentDomains_AreNotEqual()
    {
        // Arrange
        var email1 = Email.Create("usuario@gmail.com").Value;
        var email2 = Email.Create("usuario@outlook.com").Value;
        
        // Act & Assert
        Assert.IsFalse(email1.Equals(email2));
        Assert.IsFalse(email1 == email2);
        Assert.IsTrue(email1 != email2);
    }
    
    [TestMethod]
    public void Create_ValidEmailWithSubdomain_ReturnsSuccess()
    {
        // Arrange
        var validEmail = "usuario@sub.dominio.com";
        
        // Act
        var result = Email.Create(validEmail);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("usuario@sub.dominio.com", result.Value.Address);
    }
    
    [TestMethod]
    public void Create_ValidEmailWithNumbersInDomain_ReturnsSuccess()
    {
        // Arrange
        var validEmail = "usuario@dominio123.com";
        
        // Act
        var result = Email.Create(validEmail);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("usuario@dominio123.com", result.Value.Address);
    }
    
    [TestMethod]
    public void Create_ValidEmailWithLongTLD_ReturnsSuccess()
    {
        // Arrange
        var validEmail = "usuario@dominio.international";
        
        // Act
        var result = Email.Create(validEmail);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("usuario@dominio.international", result.Value.Address);
    }
}