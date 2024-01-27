using BankingApp.Data;
using BankingApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankingApp.Repositories;

// For Data access, interact with database
public class BillPayRepository(BankingAppContext context) : IBillPayRepository
{

    // Retrieve Scheduled BillPayment for current customer
    public IEnumerable<BillPay> GetScheduledBillPaysForCustomer(int customerID)
    {
        return context.BillPays
            .Where(bp => bp.Account.CustomerID == customerID)
            .Where(bp => bp.Status == BillPayStatus.Scheduled)
            .Where(bp => bp.ScheduleTimeUtc > DateTime.UtcNow)
            .ToList();
    }
    
    // Retrieve Scheduled BillPayment for current customer
    public IEnumerable<BillPay> GetFailedBillPaysForCustomer(int customerID)
    {
        return context.BillPays
            .Where(bp => bp.Account.CustomerID == customerID)
            .Where(bp => bp.Status == BillPayStatus.Failed)
            .ToList();
    }
    
    // Retrieve bill pays need to be processed
    public IEnumerable<BillPay> GetPendingBillPays()
    {
        var now = DateTime.UtcNow;
        return context.BillPays.Where(bp => bp.Status == BillPayStatus.Scheduled && bp.ScheduleTimeUtc <= now).ToList();
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
    
    public void CancelBillPay(int billPayId)
    {
        var billPay = context.BillPays.Find(billPayId);
        if (billPay != null)
        {
            context.BillPays.Remove(billPay);
            context.SaveChanges();
        }
    }

    public void CompleteBillPay(int billPayId)
    {
        var billPay = context.BillPays.Find(billPayId);
        var account = context.Accounts.Find(billPay.AccountNumber);
        if (billPay == null)
            return;
        // Pay the bill if has enough money
        var hasEnoughMoney = (account.Balance - billPay.Amount) > account.MinimumBalanceAllowed;
        if (hasEnoughMoney)
        {
            account.Balance -= billPay.Amount;
            account.Transactions.Add( new Transaction
            {
                AccountNumber = account.AccountNumber,
                Amount = billPay.Amount,
                TransactionTimeUtc = DateTime.UtcNow,
                TransactionType = TransactionType.Billpay
            });

            switch (billPay.Period)
            {
                // Handle billpay row
                case 'O':
                    billPay.Status = BillPayStatus.Succeeded;
                    break;
                case 'M':
                    billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1);
                    billPay.Status = BillPayStatus.Scheduled;
                    break;
            }
        }
        else
        {
            billPay.Status = BillPayStatus.Failed;
            context.BillPays.Update(billPay);
        }
        context.SaveChanges();
    }
    
    public BillPay GetBillPay(int billPayId)
    {
        return context.BillPays.Find(billPayId);
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