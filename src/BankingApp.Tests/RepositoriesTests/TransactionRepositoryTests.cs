using BankingApp.Data;
using BankingApp.Models;
using BankingApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Tests.RepositoriesTests;

public class TransactionRepositoryTests : IDisposable
{
    private readonly BankingAppContext _context;

    public TransactionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BankingAppContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BankingAppContext(options);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    
    [Fact]
    public void AddTransaction_ValidTransaction_TransactionAddedSuccessfully()
    {
        // Arrange
        var transactionRepository = new TransactionRepository(_context);
        var transaction = new Transaction
        {
            TransactionType = TransactionType.Deposit,
            AccountNumber = 1234,
            Amount = 100,
            Comment = "Test transaction",
            TransactionTimeUtc = DateTime.UtcNow
        };

        // Act
        transactionRepository.AddTransaction(transaction);

        // Assert
        var addedTransaction = _context.Transactions.FirstOrDefault(t => t.AccountNumber == 1234);
        Assert.NotNull(addedTransaction);
        Assert.Equal(transaction.Amount, addedTransaction.Amount);
        Assert.Equal(transaction.Comment, addedTransaction.Comment);
    }

    [Fact]
    public void GetTransactionsForAccount_SuccessfullyReturnTransaction()
    {
        // Arrange
        var transactionRepository = new TransactionRepository(_context);
        var accountNumber = 1111;
        var transactions = new List<Transaction>
        {
            new Transaction { AccountNumber = accountNumber, TransactionType = TransactionType.Deposit,Amount = 100, TransactionTimeUtc = DateTime.UtcNow },
            new Transaction { AccountNumber = accountNumber, TransactionType = TransactionType.Withdraw,Amount = 200, TransactionTimeUtc = DateTime.UtcNow }
        };

        _context.Transactions.AddRange(transactions);
        _context.SaveChanges();

        // Act
        var retrievedTransactions = transactionRepository.GetTransactionsForAccount(accountNumber);

        // Assert
        Assert.NotNull(retrievedTransactions);
        Assert.Equal(transactions.Count, retrievedTransactions.Count());
        foreach (var transaction in transactions)
        {
            Assert.Contains(transaction, retrievedTransactions);
        }
    }

    [Fact]
    public void GetTransactionsForAccount_NonExistingAccount_ReturnsEmptyList()
    {
        // Arrange
        var transactionRepository = new TransactionRepository(_context);
        var nonExistingAccountNumber = 999;

        // Act
        var transactions = transactionRepository.GetTransactionsForAccount(nonExistingAccountNumber);

        // Assert
        Assert.Empty(transactions);
    }
}