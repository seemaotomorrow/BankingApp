using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankingApp.Models;

public enum TransactionType
{
    Deposit = 'D', 
    Withdraw = 'W',
    TransferOut = 'O',
    TransferIn ='I',
    ServiceCharge = 'S',
    Billpay = 'B'
}

public class Transaction
{
    public int TransactionID { get; set; }
    
    [Required]
    public TransactionType TransactionType { get; set; }
    
    [ForeignKey("Account")]
    [Required]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }
    
    [ForeignKey("DestinationAccount")]
    public int? DestinationAccountNumber { get; set; }
    public virtual Account DestinationAccount { get; set; }
    
    [Column(TypeName = "money")]
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue,ErrorMessage = "Amount must be a positive value bigger than 0.01")]
    public decimal Amount { get; set; } 
    
    [StringLength(30, ErrorMessage = "Comment cannot exceed 30 characters")]
    public string? Comment { get; set; }
    
    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime TransactionTimeUtc { get; set; }
}





