using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BankingApp.Models;
using BankingApp.Utilities;
using BankingApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BankingApp.Filters;
using Microsoft.AspNetCore.Authorization;

namespace BankingApp.Controllers;

    [AuthorizeCustomer]  
    public class MyStatementsController : Controller
    {
        private readonly BankingAppContext _context;

        public MyStatementsController(BankingAppContext context)
        {
            _context = context;
        }  

        
    public async Task<IActionResult> Index(int accountId, int page = 1)
    {
        // const int PageSize = 4;
        // var loggedInCustomerId = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.Identity.Name : null;
    
        // var account = await _context.Accounts
        //     .Include(a => a.Customer)
        //     .FirstOrDefaultAsync(a => a.AccountId == accountId);
        //
        // if (account == null || account.CustomerID.ToString() != loggedInCustomerId)
        // {
             return NotFound();  
        // }
        //
        // var transactionsQuery = _context.Transactions
        //     .Where(t => t.AccountId == accountId)
        //     .OrderByDescending(t => t.TransactionTimeUtc);
        //
        // var paginatedList = await PaginatedList<Transaction>.CreateAsync(transactionsQuery, page, PageSize);
        // ViewData["CurrentBalance"] = account.CurrentBalance;
        // return View(paginatedList);
    
    }
}