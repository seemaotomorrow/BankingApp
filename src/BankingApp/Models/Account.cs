using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.Models;

public enum  AccountType
{
    Checking = 'C', //or change it to num 1/2
    Saving = 'S'
}

public class Account
{
    // [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    [Required]
    [Range(1000, 9999, ErrorMessage = "Account number must be 4 digits")]
    public int AccountNumber { get; set; }
    
    [Required]
    [StringLength(1)]
    [RegularExpression("^[CS]$", ErrorMessage = "AccountType must be c or s")]
    public char AccountType { get; set; }
    
    [Required]
    public int CustomerID { get; set; }
    
    [ForeignKey("Customer")]
    public virtual Customer Customer { get; set; }
    
}