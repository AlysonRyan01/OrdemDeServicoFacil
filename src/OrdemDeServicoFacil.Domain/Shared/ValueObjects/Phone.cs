using System.Text.RegularExpressions;
using OrdemDeServicoFacil.Domain.Shared.Models;

namespace OrdemDeServicoFacil.Domain.Shared.ValueObjects;

public class Phone : ValueObject
{
    public string Number { get; private set; }

    private Phone(string number)
    {
        Number = Regex.Replace(number, "[^0-9]", "");
    }

    public static Result<Phone> Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Result<Phone>.Fail("Telefone não pode ser vazio");

        var digitsOnly = Regex.Replace(phone, "[^0-9]", "");

        if (digitsOnly.Length < 10 || digitsOnly.Length > 11)
            return Result<Phone>.Fail("Telefone deve conter 10 ou 11 dígitos");
        
        var ddd = digitsOnly[..2];
        var validDdds = new[] { 
            "11", "21", "31", "41", "51", "61", "71", "81", "91", 
            "12", "22", "32", "42", "52", "62", "72", "82", "92",
            "13", "23", "33", "43", "53", "63", "73", "83", "93",
            "14", "24", "34", "44", "54", "64", "74", "84", "94",
            "15", "25", "35", "45", "55", "65", "75", "85", "95",
            "16", "26", "36", "46", "56", "66", "76", "86", "96",
            "17", "27", "37", "47", "57", "67", "77", "87", "97",
            "18", "28", "38", "48", "58", "68", "78", "88", "98",
            "19", "29", "39", "49", "59", "69", "79", "89", "99" };
    
        if (!validDdds.Contains(ddd))
            return Result<Phone>.Fail("DDD inválido");

        return Result<Phone>.Success(new Phone(digitsOnly));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    public override string ToString()
        => Number.Length == 10 
            ? $"({Number[..2]}) {Number.Substring(2, 4)}-{Number.Substring(6, 4)}"
            : $"({Number[..2]}) {Number.Substring(2, 5)}-{Number.Substring(7, 4)}";
}