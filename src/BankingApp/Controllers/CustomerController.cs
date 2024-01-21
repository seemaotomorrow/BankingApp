using BankingApp.Data;
using BankingApp.Models;
using BankingApp.ViewModels;
using BankingApp.Filters;
using BankingApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace BankingApp.Controllers;

[AuthorizeCustomer]
public class CustomerController : Controller
{

    private readonly BankingAppContext _context;
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(BankingAppContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Eager loading.
        var customer = await _context.Customers.Include(x => x.Accounts).
            FirstOrDefaultAsync(x => x.CustomerID == CustomerID);
        return View(customer);
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
        var account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
        // validators
        if(viewModel.Amount <= 0)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount must be positive.");
        }
        else if(viewModel.Amount.HasMoreThanTwoDecimalPlaces())
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount cannot have more than 2 decimal places.");
        }
        else if (!string.IsNullOrEmpty(viewModel.Comment) && viewModel.Comment.Length > 30)
        {
            ModelState.AddModelError(nameof(viewModel.Comment), "Comment exceeded maximum length of 30 characters.");
        }

        if (!ModelState.IsValid)
            return View(viewModel);
        
        account.Balance += viewModel.Amount;
        
        account.Transactions.Add(
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                AccountNumber = viewModel.AccountNumber,
                Amount = viewModel.Amount,
                Comment = viewModel.Comment,
                TransactionTimeUtc = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
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

        // Validators (similar to deposit)
        if (viewModel.Amount <= 0)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount must be positive.");
        }
        else if (viewModel.Amount.HasMoreThanTwoDecimalPlaces())
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount cannot have more than 2 decimal places.");
        }
        else if (!string.IsNullOrEmpty(viewModel.Comment) && viewModel.Comment.Length > 30)
        {
            ModelState.AddModelError(nameof(viewModel.Comment), "Comment exceeded maximum length of 30 characters.");
        }

        if (!ModelState.IsValid)
            return View(viewModel);

        // Perform withdrawal logic
        if (account.Balance < viewModel.Amount)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient funds for withdrawal.");
            return View(viewModel);
        }

        account.Balance -= viewModel.Amount;

        account.Transactions.Add(new Transaction
        {
            TransactionType = TransactionType.Withdraw,
            AccountNumber = viewModel.AccountNumber,
            Amount = viewModel.Amount,
            Comment = viewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
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
        if (viewModel.Amount <= 0)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount must be positive.");
        }
        else if (viewModel.Amount.HasMoreThanTwoDecimalPlaces())
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount cannot have more than 2 decimal places.");
        }
        else if (viewModel.SourceAccountNumber == viewModel.DestinationAccountNumber)
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
        
        // if (sourceAccount == destinationAccount)
        // {
        //     ModelState.AddModelError("", "Source account and destination account cannot be same.");
        //     return View(viewModel);
        // }

        if (sourceAccount.Balance < viewModel.Amount)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Insufficient funds for transfer.");
            return View(viewModel);
        }
        
        // Perform transfer logic
        sourceAccount.Balance -= viewModel.Amount;
        destinationAccount.Balance += viewModel.Amount;

        sourceAccount.Transactions.Add(new Transaction
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
            AccountNumber = viewModel.DestinationAccountNumber,
            Amount = viewModel.Amount,
            Comment = viewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
