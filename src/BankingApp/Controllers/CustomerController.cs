using BankingApp.Data;
using BankingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers;

public class CustomerController : Controller
{
    private readonly BankingAppContext _context;

    public CustomerController(BankingAppContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<Account> accounts = _context.Accounts.ToList();

        return View(accounts);
    }

    // Edit customer info
    // public async Task<IActionResult> Edit(int? customerID)
    // {
    //     if (customerID == null)
    //         return NotFound();
    //     var customer = await _context.Customers.FindAsync(customerID);
    //     if (customer == null)
    //         return NotFound();
    //     return View(customer);
    // }
    //
    // Change password
}