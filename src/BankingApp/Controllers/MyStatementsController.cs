using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using BankingApp.Models;
using BankingApp.Utilities;
using BankingApp.Data;
namespace BankingApp.Controllers;

public class MyStatementsController : Controller
{
    private readonly BankingAppContext _context;

    public MyStatementsController(BankingAppContext context)
    {
        _context = context;
    }  
    
    //Get statements
    // public async Task<IActionResult> Index(int accountId, int page = 1)
    // {
        // const int PageSize = 4;
        // var account = await _context.Accounts.FindAsync(accountId);

        // if (account == null)
        // {
            // return NotFound();
        // }
        
        // var transactionsQuery = _context.Transactions
            // .Where(t => t.AccountId == accountId)
            // .OrderByDescending(t => t.TransactionTimeUtc);

        // var paginatedList = await PaginatedList<Transaction>.CreateAsync(transactionsQuery, page, PageSize);
        // ViewData["CurrentBalance"] = account.CurrentBalance;
        // return View(paginatedList);
        
    // }
}