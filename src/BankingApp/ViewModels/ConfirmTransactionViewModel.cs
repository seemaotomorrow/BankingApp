using BankingApp.Models;

namespace BankingApp.ViewModels;

public class ConfirmTransactionViewModel
{
    public TransactionType TransactionType { get; set; }
    public int SourceAccountNumber { get; set; }
    public AccountType SourceAccountType { get; set; }
    public int? DestinationAccountNumber { get; set; }
    public decimal Amount { get; set; }
    public string? Comment { get; set; }
    public decimal? ServiceCharge { get; set; }
    public bool ApplyServiceCharge { get; set; }
    
}