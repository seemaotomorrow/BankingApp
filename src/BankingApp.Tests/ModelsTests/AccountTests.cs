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
    
    [Fact]
    public void HasFreeTransaction_ReturnsTrueWhenFreeTransactionsAvailable()
    {
        // Arrange
        var account = new Account();
        account.Transactions = new List<Transaction>
        {
            new Transaction { TransactionType = TransactionType.Withdraw },
        };

        // Act
        var result = account.HasFreeTransaction();

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void NoFreeTransaction_ReturnsFalseWhenFreeTransactionsUseUP()
    {
        // Arrange
        var account = new Account();
        account.Transactions = new List<Transaction>
        {
            new Transaction { TransactionType = TransactionType.Withdraw },
            new Transaction {TransactionType = TransactionType.TransferOut}
        };

        // Act
        var result = account.HasFreeTransaction();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ApplyServiceCharge_CreatesTransactionWhenServiceChargeApplied()
    {
        // Arrange
        var account = new Account { AccountNumber = 1234 };
        var amount = 10.0M;
        var applyServiceCharge = true;

        // Act
        var transaction = account.ApplyServiceCharge(amount, applyServiceCharge);

        // Assert
        Assert.NotNull(transaction);
        Assert.Equal(TransactionType.ServiceCharge, transaction.TransactionType);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(account.AccountNumber, transaction.AccountNumber);
        Assert.True(Math.Abs((DateTime.UtcNow - transaction.TransactionTimeUtc).TotalSeconds) < 1); // Within 1 second accuracy
    }
    
    [Fact]
    public void ApplyServiceCharge_NoTransactionWhenNoServiceChargeApplied()
    {
        // Arrange
        var account = new Account { AccountNumber = 1234 };
        var amount = 0M;
        var applyServiceCharge = false;

        // Act
        var transaction = account.ApplyServiceCharge(amount, applyServiceCharge);

        // Assert
        Assert.Null(transaction);
    }
}