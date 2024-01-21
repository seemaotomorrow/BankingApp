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
    
}
