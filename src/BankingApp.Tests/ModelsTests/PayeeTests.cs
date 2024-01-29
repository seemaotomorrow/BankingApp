using System.ComponentModel.DataAnnotations;
using BankingApp.Models;

namespace BankingApp.Tests.ModelsTests;

public class PayeeTests
{
    [Theory]
    [InlineData("Payee Name", "Sample Address", "Sample City", "NSW", "1234", "(02) 1234 5678")]
    public void Payee_ValidParameters_Success(string name, string address, string city, string state, string postcode, string phone)
    {
        // Arrange
        var payee = new Payee
        {
            Name = name,
            Address = address,
            City = city,
            State = state,
            Postcode = postcode,
            Phone = phone
        };

        // Act
        var validationResults = ValidateModel(payee);

        // Assert
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData("", "Sample Address", "Sample City", "NSW", "1234", "(02) 1234 5678")]
    [InlineData("Payee Name", "", "Sample City", "NSW", "1234", "(02) 1234 5678")]
    [InlineData("Payee Name", "Sample Address", "", "NSW", "1234", "(02) 1234 5678")]
    [InlineData("Payee Name", "Sample Address", "Sample City", "", "1234", "(02) 1234 5678")]
    [InlineData("Payee Name", "Sample Address", "Sample City", "NSW", "", "(02) 1234 5678")]
    [InlineData("Payee Name", "Sample Address", "Sample City", "NSW", "1234", "")]
    [InlineData("Payee Name", "Sample Address", "Sample City", "NSW", "12345", "(02) 1234 5678")]
    [InlineData("Payee Name", "Sample Address", "Sample City", "NSW", "1234", "(2) 1234 5678")]
    [InlineData("Payee Name", "Sample Address", "Sample City", "NSW", "1234", "041234567")]
    [InlineData("Payee Name", "Sample Address", "Sample City", "NSW", "12345", "441234 567")]
    public void Payee_InvalidParameters_Fail(string name, string address, string city, string state, string postcode, string phone)
    {
        // Arrange
        var payee = new Payee
        {
            Name = name,
            Address = address,
            City = city,
            State = state,
            Postcode = postcode,
            Phone = phone
        };

        // Act
        var validationResults = ValidateModel(payee);

        // Assert
        Assert.NotEmpty(validationResults);
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}