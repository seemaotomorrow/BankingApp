using System.ComponentModel.DataAnnotations;
using BankingApp.Models;

namespace BankingApp.Tests.ModelsTests;

public class LoginTests
{
    [Theory]
    [InlineData("12345678", "password")]
    [InlineData("09876543", "passwordhased")]
    public void Login_ValidParameters_Succeed(string loginID, string password)
    {
        // Arrange
        var login = new Login
        {
            LoginID = loginID,
            PasswordHash = password
        };        
        var validationContext = new ValidationContext(login);
        var validationResults = new List<ValidationResult>();
        
        // Act
        Validator.TryValidateObject(login, validationContext, validationResults, true);
        
        // Assert
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData("random88", "passwordhash")]
    [InlineData("999", "passwordhash")]
    [InlineData("loginids", "passwordhash")]
    [InlineData("-1234567", "passwordhash")]
    public void Login_InvalidLoginID_Fail(string loginID, string password)
    {
        // Arrange
        var login = new Login
        {
            LoginID = loginID,
            PasswordHash = password
        };        
        var validationContext = new ValidationContext(login);
        var validationResults = new List<ValidationResult>();
        
        // Act
        Validator.TryValidateObject(login, validationContext, validationResults, true);
        
        // Assert
        var result = Assert.Single(validationResults);
        Assert.Equal("LoginID must be 8 digits", result.ErrorMessage);
    }
    
    [Theory]
    [InlineData("12345678", null)]
    public void Login_EmptyPassword_Fail(string loginID, string password)
    {
        // Arrange
        var login = new Login
        {
            LoginID = loginID,
            PasswordHash = password
        };        
        var validationContext = new ValidationContext(login);
        var validationResults = new List<ValidationResult>();
        
        // Act
        Validator.TryValidateObject(login, validationContext, validationResults, true);
        
        // Assert
        var result = Assert.Single(validationResults);
        Assert.Equal("The PasswordHash field is required.", result.ErrorMessage);
    }
}