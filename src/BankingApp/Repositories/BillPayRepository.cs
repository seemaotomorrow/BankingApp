using BankingApp.Data;
using BankingApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankingApp.Repositories;

// For Data access, interact with database
public class BillPayRepository(BankingAppContext context) : IBillPayRepository
{
    private readonly BankingAppContext _context = context;

    public IEnumerable<BillPay> GetBillPaysForCustomer(int customerID)
    {
        // Retrieve all bill payments for a specific customer
        return context.BillPays
            .Where(bp => bp.Account.CustomerID == customerID)
            .ToList();
    }
    
    public int ScheduleBillPay(int accountNumber, int payeeId, decimal amount, DateTime scheduleTimeUtc, char period)
    {
        var billPay = new BillPay
        {
            AccountNumber = accountNumber,
            PayeeID = payeeId,
            Amount = amount,
            ScheduleTimeUtc = scheduleTimeUtc,
            Period = period,
            Status = BillPayStatus.Scheduled
        };

        context.BillPays.Add(billPay);
        context.SaveChanges();

        return billPay.BillPayID;
    }

    public BillPay GetBillPay(int billPayId)
    {
        return context.BillPays.Find(billPayId);
    }
    
    public void CancelBillPay(int billPayId)
    {
        var billPay = _context.BillPays.Find(billPayId);
        if (billPay != null)
        {
            _context.BillPays.Remove(billPay);
            _context.SaveChanges();
        }
    }

    public void CompleteBillPay(int billPayId)
    {
        var billPay = _context.BillPays.Find(billPayId);
        if (billPay != null)
        {
        }
    }

    public IEnumerable<BillPay> GetPendingBillPays()
    {
        var now = DateTime.UtcNow;
        return _context.BillPays.Where(bp => bp.Status == BillPayStatus.Pending && bp.ScheduleTimeUtc <= now).ToList();
    }
    
    // Consider put these method somewhere else
    public SelectList GetAccountNumbersForCurrentCustomer (int customerID)
    {
        var accountNumbersList = context.Accounts
            .Where(account => account.CustomerID == customerID)
            .Select(account => account.AccountNumber)
            .ToList();
        var accountNumbers = new SelectList(accountNumbersList);

        return accountNumbers;
    }

    public SelectList GetPayeeIDs()
    {
        var payeeIDsList = context.Payees.Select(payee => payee.PayeeID).ToList();
        var payeeIDs = new SelectList(payeeIDsList);
        return payeeIDs;

    }
}