using Microsoft.AspNetCore.Mvc;
using BankingApp.Models;
using BankingApp.Filters;
using BankingApp.Repositories;
using BankingApp.ViewModels;

namespace BankingApp.Controllers;

[AuthorizeCustomer]
public class BillPayController(IBillPayRepository billPayRepository) : Controller
{
    private readonly IBillPayRepository _billPayRepository = billPayRepository;
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    // show all BillPay record
    public async Task<IActionResult> Index()
    {
        var billPays = billPayRepository.GetBillPaysForCustomer(CustomerID);
        return View(billPays);
    }
    
    public IActionResult Create()
    {
        var accountNumbers = billPayRepository.GetAccountNumbersForCurrentCustomer(CustomerID);
        var payeeIDs = billPayRepository.GetPayeeIDs();
        return View(
            new CreateBillPayViewModel
            {
                AccountNumbers = accountNumbers,
                PayeeIDs = payeeIDs
            });
    }
    
    // Schedule new billPay
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateBillPayViewModel model)
    {
        // data annotation validation
        if (!ModelState.IsValid)
            return View(model);

        billPayRepository.ScheduleBillPay(model.SelectedAccountNumber, model.SelectedPayeeID, 
            model.Amount, model.ScheduleTimeUtc, model.Period); 
        
        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult CancelBillPay(int billPayId)
    {
        _billPayRepository.CancelBillPay(billPayId);
        return RedirectToAction("Index");
    }
}


// Check the bIllpay table for rows that need to be
// processed, select the row that are scheduled to be paid;
// i.e., comparing dtaTime,UtcNow to scheduletimeUtc
// - process the bill
// if not -- what should be done?
//- Opyion 1: skip it 


        
  