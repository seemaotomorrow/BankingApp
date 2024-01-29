using BankingApp.Controllers;
using BankingApp.Models;
using BankingApp.Repositories;
using BankingApp.Services;
using BankingApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace BankingApp.Tests.ControllersTests;

public class TransactionControllerTests
{
    [Fact]
    public async Task Deposit_ValidModelState_RedirectsToConfirmTransaction()
    {
        // Arrange
        var accountRepository = Substitute.For<IAccountRepository>();
        var bankingService = Substitute.For<IBankingService>();
        var controller = new TransactionController(accountRepository, bankingService);
        var viewModel = new DepositViewModel
        {
            AccountNumber = 1234,
            Amount = 100,
            Comment = "Test deposit"
        };

        // Act
        var result = await controller.Deposit(viewModel) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(TransactionController.ConfirmTransaction), result.ActionName);
    }
    
}