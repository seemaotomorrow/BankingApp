using BankingApp.Data;
using BankingApp.Models;
using BankingApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Tests.RepositoriesTests;

// Reference: RMIT WDT Day10-Lectorial
public class AccountRepositoryTests : IDisposable
{
    private readonly BankingAppContext _context;

    public AccountRepositoryTests()
    {
        _context = new BankingAppContext(new DbContextOptionsBuilder<BankingAppContext>().
            UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);

        _context.Database.EnsureCreated();

        SeedData.Initialize(_context);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
    
    
    [Fact]
    public void GetAccount_ValidAccountNumber_ReturnsAccount()
    {
        // Arrange
        var accountRepository = new AccountRepository(_context);
        _context.Accounts.Add(new Account { AccountNumber = 1234, CustomerID = 2100,Balance = 100 });
        _context.SaveChanges();
        
        // Act
        var account = accountRepository.GetAccount(1234);

        // Assert
        Assert.NotNull(account);
        Assert.Equal(100, account.Balance);
    }

    [Fact]
    public void GetAccount_NonExistingAccount_ReturnsNull()
    {
        // Arrange
        var accountRepository = new AccountRepository(_context);
        _context.Accounts.Add(new Account { AccountNumber = 1234,CustomerID = 2100, Balance = 100 });
        _context.SaveChanges();
        
        // Act
        var account = accountRepository.GetAccount(999); // Non-existing account number

        // Assert
        Assert.Null(account);
    }
    
    
    [Fact]
    public void UpdateAccount_SuccessfullyUpdateBalance()
    {
        // Arrange
        var accountRepository = new AccountRepository(_context);
        var initialBalance = 100m;
        var updatedBalance = 200m;

        var account = new Account { AccountNumber = 1234, CustomerID = 2100, Balance = initialBalance };
        _context.Accounts.Add(account);
        _context.SaveChanges();

        // Act
        var accountToUpdate = accountRepository.GetAccount(1234);
        accountToUpdate.Balance = updatedBalance;
        accountRepository.UpdateAccount(accountToUpdate);

        // Assert
        var updatedAccount = _context.Accounts.Find(1234);
        Assert.NotNull(updatedAccount);
        Assert.Equal(updatedBalance, updatedAccount.Balance);
    }
       

}
