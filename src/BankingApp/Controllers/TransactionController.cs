using Microsoft.AspNetCore.Mvc;
using BankingApp.Data;
using BankingApp.Models;
using BankingApp.Filters;
using BankingApp.Tools.Utilities;
using BankingApp.ViewModels;

namespace BankingApp.Controllers;

[AuthorizeCustomer]
public class TransactionController : Controller
{
    private readonly BankingAppContext _context;
    private const decimal AccountTransferServiceCharge = 0.10m;
    private const decimal AtmWithdrawalServiceCharge = 0.05m;
    
    public TransactionController(BankingAppContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Deposit(int accountNumber)
    {
        var account = await _context.Accounts.FindAsync(accountNumber);
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
        ValidateAmount(viewModel.Amount);
        ValidateComment(viewModel.Comment);

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
        var account = await _context.Accounts.FindAsync(accountNumber);
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
        var account = await _context.Accounts.FindAsync(viewModel.AccountNumber);

        // Validators
        ValidateAmount(viewModel.Amount);
        ValidateComment(viewModel.Comment);

        if (!ModelState.IsValid)
            return View(viewModel);

        // Validate if it's sufficient to cover service fee
        decimal serviceCharge = 0;
        bool applyServiceCharge = !(account.HasFreeTransaction());
        if (applyServiceCharge)
            serviceCharge = AtmWithdrawalServiceCharge;
        var sufficient = account.Balance - (viewModel.Amount + serviceCharge) > account.MinimumBalanceAllowed;
        if (!sufficient)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient funds for withdrawal.");
            return View(viewModel);
        }
        
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
    
    public async Task<IActionResult> Transfer(int accountNumber)
    {
        var account = await _context.Accounts.FindAsync(accountNumber);
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
        var sourceAccount = _context.Accounts.Find(viewModel.SourceAccountNumber);
        var destinationAccount = _context.Accounts.Find(viewModel.DestinationAccountNumber);

        // Validators
        ValidateAmount(viewModel.Amount);
        ValidateComment(viewModel.Comment);
        if (viewModel.SourceAccountNumber == viewModel.DestinationAccountNumber)
        {
            ModelState.AddModelError("", "Source and destination accounts cannot be the same.");
        }

        if (!ModelState.IsValid)
            return View(viewModel);
        
        if (destinationAccount == null)
        {
            ModelState.AddModelError("", "Invalid destination account.");
            return View(viewModel);
        }
        
        // Check if it's sufficient
        decimal serviceCharge = 0;
        bool applyServiceCharge = !(sourceAccount.HasFreeTransaction());
        if (applyServiceCharge)
            serviceCharge = AccountTransferServiceCharge;
        var sufficient = sourceAccount.Balance - (viewModel.Amount + serviceCharge) > sourceAccount.MinimumBalanceAllowed;
        if (!sufficient)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient funds for transfer.");
            return View(viewModel);
        }
        
        // Perform transfer logic
        sourceAccount.Balance -= (viewModel.Amount + serviceCharge);
        destinationAccount.Balance += viewModel.Amount;

        Transaction transaction = new Transaction()
        {
            TransactionType = TransactionType.TransferOut,
            AccountNumber = viewModel.SourceAccountNumber,
            DestinationAccountNumber = viewModel.DestinationAccountNumber,
            Amount = viewModel.Amount,
            Comment = viewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow
        };
        
        var serviceChargeTransaction = sourceAccount.ApplyServiceCharge(serviceCharge, applyServiceCharge);
        if(serviceChargeTransaction != null)
            sourceAccount.Transactions.Add(serviceChargeTransaction);

        return RedirectToAction(nameof(ConfirmTransaction),transaction );
    }
    
    public async Task<IActionResult> ConfirmTransaction(Transaction transaction)
    {
        var account = await _context.Accounts.FindAsync(transaction.AccountNumber);

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
        var account = await _context.Accounts.FindAsync(viewModel.SourceAccountNumber);
        
        // Process the confirmed transaction here
        if (viewModel.TransactionType == TransactionType.Deposit)
        {
            account.Balance += viewModel.Amount;
            account.Transactions.Add(new Transaction()
            {
                TransactionType = TransactionType.Deposit,
                AccountNumber = account.AccountNumber,
                Amount = viewModel.Amount,
                Comment = viewModel.Comment,
                TransactionTimeUtc = DateTime.UtcNow
            });
        }

        // Process Withdraw
        if (viewModel.TransactionType == TransactionType.Withdraw)
        {
            // Check if apply service charge
            decimal serviceCharge = 0;
            bool applyServiceCharge = !(account.HasFreeTransaction());
            if (applyServiceCharge)
                serviceCharge = AtmWithdrawalServiceCharge;
            
            account.Transactions.Add(new Transaction()
            {
                TransactionType = TransactionType.Withdraw,
                AccountNumber = account.AccountNumber,
                Amount = viewModel.Amount,
                Comment = viewModel.Comment,
                TransactionTimeUtc = DateTime.UtcNow
            });
            account.Balance -= (viewModel.Amount+ serviceCharge);
            
            var serviceChargeTransaction = account.ApplyServiceCharge(serviceCharge, applyServiceCharge);
            if(serviceChargeTransaction != null)
                account.Transactions.Add(serviceChargeTransaction);
        }

        // Process Transfer
        if (viewModel.TransactionType == TransactionType.TransferOut)
        {
            var destinationAccount = _context.Accounts.Find(viewModel.DestinationAccountNumber);

            // Check if apply service charge
            decimal serviceCharge = 0;
            bool applyServiceCharge = !(account.HasFreeTransaction());
            if (applyServiceCharge)
                serviceCharge = AccountTransferServiceCharge;
            
            // Perform transfer logic
            account.Balance -= (viewModel.Amount + serviceCharge);
            destinationAccount.Balance += viewModel.Amount;

            account.Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.TransferOut,
                AccountNumber = viewModel.SourceAccountNumber,
                DestinationAccountNumber = viewModel.DestinationAccountNumber,
                Amount = viewModel.Amount,
                Comment = viewModel.Comment,
                TransactionTimeUtc = DateTime.UtcNow
            });

            destinationAccount.Transactions.Add(new Transaction
            {
                TransactionType = TransactionType.TransferIn,
                AccountNumber = destinationAccount.AccountNumber,
                Amount = viewModel.Amount,
                Comment = viewModel.Comment,
                TransactionTimeUtc = DateTime.UtcNow
            });
            
            var serviceChargeTransaction = account.ApplyServiceCharge(serviceCharge, applyServiceCharge);
            if(serviceChargeTransaction != null)
                account.Transactions.Add(serviceChargeTransaction);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), nameof(Customer));
    }

    private void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        }
        else if(amount.HasMoreThanTwoDecimalPlaces())
        {
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        }
    }

    private void ValidateComment(string? comment)
    {
        if (!string.IsNullOrEmpty(comment) && comment.Length > 30)
        {
            ModelState.AddModelError(nameof(comment), "Comment exceeded maximum length of 30 characters.");
        }
    }
}