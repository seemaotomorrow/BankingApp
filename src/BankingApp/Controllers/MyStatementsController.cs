using System.Diagnostics;
using BankingApp.Data;
using BankingApp.Filters;
using BankingApp.Models;
using BankingApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApp.ViewModels;
using System.Threading.Tasks;

namespace BankingApp.Controllers;


[AuthorizeCustomer]
public class MyStatementsController : Controller
{
    private readonly BankingAppContext _context;
    private readonly ILogger<MyStatementsController> _logger;
    private const int PageSize = 4;
    
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public MyStatementsController(BankingAppContext context, ILogger<MyStatementsController> logger )
    {
        _context = context;
        _logger = logger;
    }

// Retrieves the account statements for the logged-in customer
public async Task<IActionResult> MyStatement(int page = 1)
{
    try
    {
        // Retrieve accounts for the given customerID
        var customer = await _context.Customers.Include(x => x.Accounts).
            FirstOrDefaultAsync(x => x.CustomerID == CustomerID);
       // return View(customer);
        
       // var accounts = await _context.Accounts
       //  .Where(a => a.CustomerID == customerID)
       //  .ToListAsync();

       if (customer == null || !customer.Accounts.Any())
       {
           return NotFound("No accounts found for the given customer.");
       }
       
       // Prepare a single IQueryable to select and order transactions for all customer's accounts
       var transactionsQuery = _context.Transactions
           .Where(t => customer.Accounts.Select(a => a.AccountNumber).Contains(t.AccountNumber))
           .OrderByDescending(t => t.TransactionTimeUtc)
           .Select(t => new TransactionViewModel
           {
               
               TransactionID = t.TransactionID,
               TransactionType = GetTransactionTypeDescription(t.TransactionType),
               AccountNumber = t.AccountNumber,
               DestinationAccountNumber = t.DestinationAccountNumber,
               Amount = t.Amount,
               TransactionTimeUtc = t.TransactionTimeUtc,
               Comment = t.Comment
           });
       

       // Apply pagination directly in the IQueryable
       var paginatedTransactions = await PaginatedList<TransactionViewModel>
           .CreateAsync(transactionsQuery,page, PageSize);

       var viewModel = new StatementViewModel
       {
           Accounts = customer.Accounts,
           CustomerID = CustomerID,
           Transactions = paginatedTransactions,
           
       };

       return View("Index", viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while retrieving account statements for customerID {CustomerID}.", CustomerID);
        
        var errorViewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ErrorMessage = "Sorry, we couldn't retrieve your account statements."
        };

        return View("Error", errorViewModel);
    }
}

    
    private static string GetTransactionTypeDescription(TransactionType transactionType)
    {
        
        return transactionType switch
        {
            TransactionType.Deposit => "Deposit",
            TransactionType.Withdraw => "Withdraw",
            TransactionType.TransferOut => "Transfer Out",
            TransactionType.TransferIn => "Transfer In",
            TransactionType.ServiceCharge => "Service Charge",
            TransactionType.Billpay => "Bill Pay",
            _ => "Unknown"
        };
    }
}




   
   