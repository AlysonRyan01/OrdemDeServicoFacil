using OrdemDeServicoFacil.Domain.Shared.ValueObjects;

namespace OrdemDeServicoFacil.Tests.Domain.Shared.ValueObjects;

[TestClass]
public class PhoneTests
{
    [TestMethod]
    public void Create_DeveFalhar_QuandoTelefoneForNuloOuVazio()
    {
        // Act
        var resultEmpty = Phone.Create("");

        // Assert
        Assert.IsFalse(resultEmpty.IsSuccess);
    }

    [TestMethod]
    public void Create_DeveRemoverCaracteresEspeciais_DoNumero()
    {
        // Arrange
        var input = "(47) 9.9999-1234";

        // Act
        var phone = Phone.Create(input).Value;

        // Assert
        Assert.AreEqual("47999991234", phone.Number);
    }

    [TestMethod]
    public void Create_DeveFalhar_QuandoQuantidadeDeDigitosForInvalida()
    {
        // Arrange
        var telefoneCurto = "12345";
        var telefoneLongo = "123456789012";

        // Act
        var resultCurto = Phone.Create(telefoneCurto);
        var resultLongo = Phone.Create(telefoneLongo);

        // Assert
        Assert.IsFalse(resultCurto.IsSuccess);
        Assert.IsFalse(resultLongo.IsSuccess);
        Assert.AreEqual("Telefone deve conter 10 ou 11 dígitos", resultCurto.Error);
    }

    [TestMethod]
    public void ToString_DeveFormatarTelefoneCom10Digitos()
    {
        // Arrange
        var phone = Phone.Create("4733334444").Value;

        // Act
        var formatted = phone.ToString();

        // Assert
        Assert.AreEqual("(47) 3333-4444", formatted);
    }

    [TestMethod]
    public void ToString_DeveFormatarTelefoneCom11Digitos()
    {
        // Arrange
        var phone = Phone.Create("47999991234").Value;

        // Act
        var formatted = phone.ToString();

        // Assert
        Assert.AreEqual("(47) 99999-1234", formatted);
    }

    [TestMethod]
    public void Phone_DeveSerIgual_QuandoNumerosForemIguais()
    {
        // Arrange
        var phone1 = Phone.Create("(47) 99999-1234").Value;
        var phone2 = Phone.Create("47 99999 1234").Value;

        // Act & Assert
        Assert.AreEqual(phone1, phone2);
    }

    [TestMethod]
    public void Phone_DeveSerDiferente_QuandoNumerosForemDiferentes()
    {
        // Arrange
        var phone1 = Phone.Create("47999991234").Value;
        var phone2 = Phone.Create("4733334444").Value;

        // Act & Assert
        Assert.AreNotEqual(phone1, phone2);
    }
}