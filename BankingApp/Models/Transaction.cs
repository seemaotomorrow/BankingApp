using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankingApp.Models;

public enum TransactionType
{
    Deposit = 'D', 
    Withdraw = 'W',
    DebitTransferFromSource = 'T',
    CreditTransferToTarget ='T',
    ServiceCharge = 'S',
    Billpay = 'B'
}

public class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionID { get; set; }
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
    [Range(0.01, double.MaxValue,ErrorMessage = "Amount must be a positive value")]
    public decimal Amount { get; set; } //Amount to credit or debit?
    
    [StringLength(30)]
    public string Comment { get; set; }
    
    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime TransactionTimeUtc { get; set; }
      
}

public class Deposit : Transaction
{
    public Deposit(decimal amount, int accountNumber, string comment)
    {
        TransactionType = 'T';
        TransactionTimeUtc = DateTime.Now;
        Amount = amount;
        AccountNumber = accountNumber;
        Comment = comment;
    }
}

public class Withdrawal : Transaction
{
    private const decimal AtmWithdrawalServiceCharge = 0.05m;
    public Withdrawal(decimal amount, int accountNumber, string? note, bool applyServiceCharge)
    {
        TransactionType = 'W';
        TransactionTimeUtc = DateTime.Now;
        Amount = amount;
        AccountNumber = accountNumber;
        Comment = comment;
        ServiceCharge = applyServiceCharge
            ? new()
            {
                AccountNumber = accountNumber,
                Amount = AtmWithdrawalServiceCharge,
                TransactionTimeUtc = TransactionTimeUtc
            }
            : null;
    }


    public decimal Total => -(Amount + (ServiceCharge?.Amount ?? 0));
    public ServiceCharge? ServiceCharge { get; }
}


public class ReceiveTransfer : Transaction
{
    public ReceiveTransfer(decimal amount, int accountNumber,string note)
    {
        TransactionType = 'T';
        TransactionTimeUtc = DateTime.Now;
        Amount = amount;
        AccountNumber = accountNumber;
        Comment = comment;
    }
}


public class ServiceCharge : Transaction
{
    public ServiceCharge()
    {
        TransactionType = 'S';
    }
}
