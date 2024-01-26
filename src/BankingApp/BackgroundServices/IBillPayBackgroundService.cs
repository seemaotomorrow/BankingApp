namespace BankingApp.BackgroundServices;

public interface IBillPayBackgroundService
{
    void ScheduleBillPay(int accountID, int payeeID, decimal amount, DateTime scheduleTimeUtc, char period);
    void CancelBillPay(int billPayId);
    void ProcessPendingBillPays();
}