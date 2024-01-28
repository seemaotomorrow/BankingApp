namespace BankingApp.BackgroundServices;

public interface IBillPayBackgroundService
{
    void ProcessPendingBillPays();
}