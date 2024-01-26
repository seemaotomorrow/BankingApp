using BankingApp.Repositories;
using Hangfire;

namespace BankingApp.BackgroundServices;

public class BillPayBackgroundService(IBackgroundJobClient backgroundJobClient, IBillPayRepository billPayRepository)
    : IBillPayBackgroundService
{
    public void ScheduleBillPay(int accountId, int payeeId, decimal amount, DateTime scheduleTimeUtc, char period)
    {
        var billPayId = billPayRepository.ScheduleBillPay(accountId, payeeId, amount, scheduleTimeUtc, period);
        backgroundJobClient.Schedule(() => ProcessBillPay(billPayId), scheduleTimeUtc);
    }

    public void CancelBillPay(int billPayId)
    {
        
        backgroundJobClient.Delete(billPayId.ToString());
        billPayRepository.CancelBillPay(billPayId);
    }
    
    public void ProcessPendingBillPays()
    {
        var pendingBillPays = billPayRepository.GetPendingBillPays();
        foreach (var billPay in pendingBillPays)
        {
            ProcessBillPay(billPay.BillPayID);
        }
    }
    
    private void ProcessBillPay(int billPayId)
    {
        var billPay = billPayRepository.GetBillPay(billPayId);
        // Add logic to process the bill payment, handle success or failure
        // Update the bill pay status in the database
        billPayRepository.CompleteBillPay(billPay.BillPayID);
    }
}