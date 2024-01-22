using BankingApp.Models;

namespace BankingApp.ViewModels;

public class TransferViewModel
{
    // For display
    public int SourceAccountNumber { get; set; }
    public AccountType AccountType { get; set; }

    // For getting user input
    public int DestinationAccountNumber { get; set; }
    public decimal Amount { get; set; }
    public string? Comment { get; set; }
    
}