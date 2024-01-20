using Microsoft.AspNetCore.Mvc;
using BankingApp.Models;
using BankingApp.Data;
using BankingApp.Utilities; 
using BankingApp.Filters;

namespace BankingApp.Controllers;

[AuthorizeCustomer]
public class CustomerController : Controller
{
    private readonly BankingContext _context;

    // ReSharper disable once PossibleInvalidOperationException
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(BankingContext context)
    {
        _context = context;
    }

    // Can add authorize attribute to actions.
    //[AuthorizeCustomer]
    public async Task<IActionResult> Index()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        var customer = await _context.Customers.FindAsync(CustomerID);


        return View(customer);
    }

    public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));

    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        else if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }

        // Business logic for deposit
        account.Balance += amount;
        account.Transactions.Add(
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        // Redirect to the Index action after successful deposit
        return RedirectToAction(nameof(Index));
    }
}
    
