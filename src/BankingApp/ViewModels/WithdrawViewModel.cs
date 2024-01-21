using BankingApp.Models;

namespace BankingApp.ViewModels;

public class WithdrawViewModel
{
    // For display
    public int AccountNumber { get; set; }
    public AccountType AccountType { get; set; }
    
    // For getting user input
    public decimal Amount { get; set; }
    public string? Comment { get; set; }
}