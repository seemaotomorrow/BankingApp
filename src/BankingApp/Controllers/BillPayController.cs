using Microsoft.AspNetCore.Mvc;
using BankingApp.Models;
using BankingApp.Data;
using System.Linq;
using System.Threading.Tasks;
namespace BankingApp.Controllers;

public class BillPayController : Controller
{
    
    private readonly BankingAppContext _context;

    public BillPayController(BankingAppContext context)
    {
        _context = context;
    }
                
    // show all BillPay record
    public async Task<IActionResult> Index()
    {
        var billPays = _context.BillPays.ToList();
        return View(billPays);
    }

    // creat new record
    public IActionResult Create()
    {
        return View();
    }

    // update new record
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AccountNumber,Amount,ScheduleTimeUtc,Period")] BillPay billPay)
    {
        if (ModelState.IsValid)
        {
            _context.Add(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(billPay);
    }

    // lea:add more 
}


        
  