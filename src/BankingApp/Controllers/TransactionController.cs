using Microsoft.AspNetCore.Mvc;
using BankingApp.Models;
using BankingApp.Filters;
using BankingApp.Repositories;
using BankingApp.Services;
using BankingApp.ViewModels;

namespace BankingApp.Controllers;

[AuthorizeCustomer]
public class TransactionController : Controller
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBankingService _bankingService;
    
    public TransactionController( IAccountRepository accountRepository, IBankingService bankingService)
    {
        _accountRepository = accountRepository;
        _bankingService = bankingService;
    }
    
    public IActionResult Deposit(int accountNumber)
    {
        var account = _accountRepository.GetAccount(accountNumber);
        return View(
            new DepositViewModel
            {
                AccountNumber = accountNumber,
                AccountType = account.AccountType
            });
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(DepositViewModel viewModel)
    {
        // validators
        var amountErrors = _bankingService.ValidateAmount(viewModel.Amount);
        var commentErrors = _bankingService.ValidateComment(viewModel.Comment);
        foreach (var error in amountErrors.Concat(commentErrors))
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        if (!ModelState.IsValid)
            return View(viewModel);
        var transaction = new Transaction()
        {
            TransactionType = TransactionType.Deposit,
            AccountNumber = viewModel.AccountNumber,
            Amount = viewModel.Amount,
            Comment = viewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow
        };
        return RedirectToAction(nameof(ConfirmTransaction), transaction);
    }
    
    public async Task<IActionResult> Withdraw(int accountNumber)
    {
        var account = _accountRepository.GetAccount(accountNumber);
        return View(
            new WithdrawViewModel
            {
                AccountNumber = accountNumber,
                AccountType = account.AccountType
            });
    }
    
    [HttpPost]
    public async Task<IActionResult> Withdraw(WithdrawViewModel viewModel)
    {
        // Validators
        var amountErrors = _bankingService.ValidateAmount(viewModel.Amount);
        var commentErrors = _bankingService.ValidateComment(viewModel.Comment);
        // Validate if it's sufficient to cover service fee
        var balanceSufficientError =
            _bankingService.ValidateBalanceSufficientToCoverServiceFee(viewModel.AccountNumber, viewModel.Amount, TransactionType.Withdraw);
        foreach (var error in amountErrors.Concat(commentErrors).Concat(balanceSufficientError))
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        
        if (!ModelState.IsValid)
            return View(viewModel);
        
        // Pass data to ConfirmTransaction view model
        Transaction transaction = new Transaction()
        {
            TransactionType = TransactionType.Withdraw,
            AccountNumber = viewModel.AccountNumber,
            Amount = viewModel.Amount,
            Comment = viewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow
        };
        return RedirectToAction(nameof(ConfirmTransaction), transaction);
    }
    
    public IActionResult Transfer(int accountNumber)
    {
        var account = _accountRepository.GetAccount(accountNumber);
        return View(
            new TransferViewModel()
            {
               SourceAccountNumber = accountNumber,
                AccountType = account.AccountType
            });
    }

    [HttpPost]
    public async Task<IActionResult> Transfer(TransferViewModel viewModel)
    {
        // Validators
        var amountErrors = _bankingService.ValidateAmount(viewModel.Amount);
        var commentErrors = _bankingService.ValidateComment(viewModel.Comment);
        var balanceSufficientError =
            _bankingService.ValidateBalanceSufficientToCoverServiceFee(viewModel.SourceAccountNumber, viewModel.Amount, TransactionType.TransferOut);
        var sourceAndDestinationSameError =
            _bankingService.ValidateSourceAccAndDestinationAccDifferent(viewModel.SourceAccountNumber,
                viewModel.DestinationAccountNumber);
        var destinationAccountError = _bankingService.ValidateDestinationAccount(viewModel.DestinationAccountNumber);
        
        foreach (var error in amountErrors.Concat(commentErrors).Concat(balanceSufficientError).Concat(sourceAndDestinationSameError).Concat(destinationAccountError))
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        if (!ModelState.IsValid)
            return View(viewModel);

        var transaction = new Transaction()
        {
            TransactionType = TransactionType.TransferOut,
            AccountNumber = viewModel.SourceAccountNumber,
            DestinationAccountNumber = viewModel.DestinationAccountNumber,
            Amount = viewModel.Amount,
            Comment = viewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow
        };

        return RedirectToAction(nameof(ConfirmTransaction),transaction );
    }
    
    public async Task<IActionResult> ConfirmTransaction(Transaction transaction)
    {
        var account = _accountRepository.GetAccount(transaction.AccountNumber);

        return View(
            new ConfirmTransactionViewModel()
            {
                TransactionType = transaction.TransactionType,
                SourceAccountType = account.AccountType,
                SourceAccountNumber = transaction.AccountNumber,
                DestinationAccountNumber = transaction.DestinationAccountNumber,
                Amount = transaction.Amount,
                Comment = transaction.Comment
            });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmTransaction(ConfirmTransactionViewModel viewModel)
    {
        var account = _accountRepository.GetAccount(viewModel.SourceAccountNumber);
        
        // Process the confirmed transaction here
        if (viewModel.TransactionType == TransactionType.Deposit)
        {
            _bankingService.Deposit(account.AccountNumber, viewModel.Amount, viewModel.Comment);
        }

        // Process Withdraw
        if (viewModel.TransactionType == TransactionType.Withdraw)
        {
            _bankingService.Withdraw(account.AccountNumber,viewModel.Amount, viewModel.Comment);
        }

        // Process Transfer
        if (viewModel.TransactionType == TransactionType.TransferOut)
        {
            _bankingService.Transfer(viewModel.SourceAccountNumber, viewModel.DestinationAccountNumber.Value, viewModel.Amount, viewModel.Comment);
        }
        
        return RedirectToAction(nameof(Index), nameof(Customer));
    }
}