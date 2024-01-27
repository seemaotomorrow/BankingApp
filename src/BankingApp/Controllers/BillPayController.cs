using BankingApp.BackgroundServices;
using Microsoft.AspNetCore.Mvc;
using BankingApp.Models;
using BankingApp.Filters;
using BankingApp.Repositories;
using BankingApp.ViewModels;
using Hangfire;

namespace BankingApp.Controllers;

[AuthorizeCustomer]
public class BillPayController(IBillPayRepository billPayRepository) : Controller
{
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    // show all BillPay record
    public async Task<IActionResult> Index()
    {
        var billPays = billPayRepository.GetScheduledBillPaysForCustomer(CustomerID);
        var failedBillPays = billPayRepository.GetFailedBillPaysForCustomer(CustomerID);
        // Combine both lists
        var allBillPays = billPays.Concat(failedBillPays);
        return View(allBillPays);
    }
    
    public IActionResult Create()
    {
        var accountNumbers = billPayRepository.GetAccountNumbersForCurrentCustomer(CustomerID);
        var payeeIDs = billPayRepository.GetPayeeIDs();
        
        // Initialize the view model with dropdown list data
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
        {
            // If model state is not valid, re-fetch dropdown list data
            var accountNumbers = billPayRepository.GetAccountNumbersForCurrentCustomer(CustomerID);
            var payeeIDs = billPayRepository.GetPayeeIDs();

            // Update the model with dropdown list data
            model.AccountNumbers = accountNumbers;
            model.PayeeIDs = payeeIDs;
            
            return View(model);
        }

        var utcTime = model.ScheduleTimeUtc.ToUniversalTime();
        
        billPayRepository.ScheduleBillPay(model.SelectedAccountNumber, model.SelectedPayeeID, 
            model.Amount, utcTime, model.Period);
        
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Cancel(int billPayID)
    {
        if (billPayID == null)
            return NotFound();
        var billPayment = billPayRepository.GetBillPay(billPayID);
        
        return View(billPayment);
    }
    
    [HttpPost, ActionName("CancelConfirmed")]
    [ValidateAntiForgeryToken]
    public IActionResult CancelConfirmed(int billPayID)
    {
        billPayRepository.CancelBillPay(billPayID);

        return RedirectToAction("Index");
    }
}

// processed, select the row that are scheduled to be paid;
// i.e., comparing dtaTime,UtcNow to scheduletimeUtc
// - process the bill
// if not -- what should be done?
//- Opyion 1: skip it 


        
  