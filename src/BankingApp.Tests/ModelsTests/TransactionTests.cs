using System.ComponentModel.DataAnnotations;
using BankingApp.Models;

namespace BankingApp.Tests.ModelsTests;

public class TransactionTests
{
    private const string InvalidComment = "This is a string which is longer than 30 characters";
    
    [Fact]
    public void Transaction_ValidParameters_Success()
    {
        // Arrange
        var transaction = new Transaction
        {
            TransactionType = TransactionType.Deposit,
            AccountNumber = 1010,
            DestinationAccountNumber = null,
            Amount = 100.50m,
            Comment = "Test deposit",
            TransactionTimeUtc = DateTime.UtcNow
        };
        
        // Act
        var validationResults = ValidateModel(transaction);

        // Assert
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(TransactionType.Withdraw, 1001, null, -50.25, "Negative withdrawal", "2023-01-01T00:00:00")]
    [InlineData(TransactionType.TransferOut, 1001, 1002, 0, "Zero transfer", "2023-01-01T00:00:00")]
    [InlineData(TransactionType.TransferOut, 1001, 1002, 0.005, null, "2023-01-01T00:00:00")]
    public void Transaction_InvalidAmount_Fail(TransactionType type, int accountNumber, int? destinationAccountNumber, decimal amount, string comment, string transactionTimeUtcString)
    {
        // Arrange
        var transaction = new Transaction
        {
            TransactionType = type,
            AccountNumber = accountNumber,
            DestinationAccountNumber = destinationAccountNumber,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.Parse(transactionTimeUtcString)
        };
        
        // Act
        var validationResults = ValidateModel(transaction);

        // Assert
        var result = Assert.Single(validationResults);
        Assert.Equal("Amount must be a positive value bigger than 0.01", result.ErrorMessage);
    }
    
    [Theory]
    [InlineData(TransactionType.Withdraw, 1001, null, 50.25, InvalidComment, "2023-01-01T00:00:00")]
    public void Transaction_InvalidComment_Fail(TransactionType type, int accountNumber, int? destinationAccountNumber, decimal amount, string comment, string transactionTimeUtcString)
    {
        // Arrange
        var transaction = new Transaction
        {
            TransactionType = type,
            AccountNumber = accountNumber,
            DestinationAccountNumber = destinationAccountNumber,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.Parse(transactionTimeUtcString)
        };
        
        // Act
        var validationResults = ValidateModel(transaction);

        // Assert
        var result = Assert.Single(validationResults);
        Assert.Equal("Comment cannot exceed 30 characters", result.ErrorMessage);
    }

    
    private static IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
    
}
