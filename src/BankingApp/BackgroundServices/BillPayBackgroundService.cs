using BankingApp.Repositories;

namespace BankingApp.BackgroundServices;

public class BillPayBackgroundService(IBillPayRepository billPayRepository)
    : IBillPayBackgroundService
{
    public void ProcessPendingBillPays()
    {
        var pendingBillPays = billPayRepository.GetPendingBillPays();
        foreach (var billPay in pendingBillPays)
        {
            billPayRepository.CompleteBillPay(billPay.BillPayID);
        }
    }
}