using BankingApp.Data;
using BankingApp.Models;
using BankingApp.ViewModels;
using BankingApp.Filters;
using BankingApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
    return View(
    new DepositViewModel
    {
    AccountNumber = accountNumber,
    Account = await _context.Accounts.FindAsync(accountNumber) 
    });
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(DepositViewModel viewModel)
    {
        viewModel.Account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
        // validators
        if(viewModel.Amount <= 0)
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount must be positive.");
            return View(viewModel);
        }
        if(viewModel.Amount.HasMoreThanTwoDecimalPlaces())
        {
            ModelState.AddModelError(nameof(viewModel.Amount), "Amount cannot have more than 2 decimal places.");
            return View(viewModel);
        }
        viewModel.Account.Balance += viewModel.Amount;
        viewModel.Account.Transactions.Add(
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                AccountNumber = viewModel.AccountNumber,
                Amount = viewModel.Amount,
                TransactionTimeUtc = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
