using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebAPI.Models;

public class BillPay
{
    //memo for lea: is an attribute in EF that specific how the database generates values for a particular property.
    public int BillPayID { get; set; }
    
    [ForeignKey("Account")]
    [Required]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }
    
    [ForeignKey("Payee")]
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
    [StringLength(1)]
    [RegularExpression("^[OM]$", ErrorMessage = "Period must be 'O' or 'M'")]
    //To represent whether the payment is a one-off or monthly payment
    public char Period { get; set; } //  (One-off 'O' or Monthly 'M')


}
    
