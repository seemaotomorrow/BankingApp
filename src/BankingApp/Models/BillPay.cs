using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.Models;

public enum BillPayStatus
{
    // the bill payment has been scheduled but has not yet been processed.
    Scheduled = 1,
    Succeeded = 2,
    Failed = 3,
    // Canceled = 4
}

public class BillPay
{
    public int BillPayID { get; set; }
    
    [ForeignKey("Account")]
    [Required]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }
    
    public int PayeeID { get; set; } 
    public virtual Payee Payee { get; set; }
    
    [Column(TypeName = "money")]
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value")]
    public decimal Amount { get; set; } // Amount of funds to be withdrawn from the account

    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime ScheduleTimeUtc { get; set; } // Next scheduled date and time for the transaction to occur

    [Required(ErrorMessage = "Period is required")]
    [RegularExpression("^[OM]$", ErrorMessage = "Period must be 'O' or 'M'")]
    public char Period { get; set; } //  (One-off 'O' or Monthly 'M')
    
    public BillPayStatus Status { get; set; }
}
    
