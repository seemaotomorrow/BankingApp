using BankingApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankingApp.Repositories;

public interface IBillPayRepository
{
    int ScheduleBillPay(int accountNumber, int payeeId, decimal amount, DateTime scheduleTimeUtc, char period);
    BillPay GetBillPay(int billPayId);
    void CancelBillPay(int billPayId);
    void CompleteBillPay(int billPayId);
    IEnumerable<BillPay> GetScheduledBillPaysForCustomer(int customerID);
    IEnumerable<BillPay> GetPendingBillPays();
    IEnumerable<BillPay> GetFailedBillPaysForCustomer(int customerID);


    SelectList GetAccountNumbersForCurrentCustomer(int customerID);
    SelectList GetPayeeIDs();

}