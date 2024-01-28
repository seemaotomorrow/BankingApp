using System.ComponentModel.DataAnnotations;
using BankingApp.Models;

namespace BankingApp.Tests.ModelsTests;

public class AccountTests
{
    [Theory]
    [InlineData(1234, AccountType.Checking)]
    [InlineData(1000, AccountType.Saving)]
    [InlineData(9999, AccountType.Checking)]
    public void Account_ValidParameters_Success(int accountNumber, AccountType accountType)
    {
        // Arrange
        var account = new Account
        {
            AccountNumber = accountNumber,
            AccountType = accountType
        };

        var validationContext = new ValidationContext(account);
        var validationResults = new List<ValidationResult>();
        
        // Act
        Validator.TryValidateObject(account, validationContext, validationResults, true);
        
        // Assert
        Assert.Empty(validationResults);
    }
    
    [Theory]
    [InlineData(1, AccountType.Saving)]
    [InlineData(123, AccountType.Checking)]
    [InlineData(0000, AccountType.Checking)]
    [InlineData(500000, AccountType.Saving)]
    [InlineData(-1234, AccountType.Saving)]
    public void Account_InvalidAccountNumber_Fail(int accountNumber, AccountType accountType)
    {
        // Arrange
        var account = new Account
        {
            AccountNumber = accountNumber,
            AccountType = accountType
        };

        var validationContext = new ValidationContext(account);
        var validationResults = new List<ValidationResult>();
        
        // Act
        Validator.TryValidateObject(account, validationContext, validationResults, true);
        
        //Assert
        var result = Assert.Single(validationResults);
        Assert.Equal("Account number must be 4 digits", result.ErrorMessage);
    }
}