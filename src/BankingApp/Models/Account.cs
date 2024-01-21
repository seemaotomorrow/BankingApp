using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BankingApp.Utilities;

namespace BankingApp.Models;

public enum AccountType
{
    Checking = 1,
    Saving = 2
}

public class Account
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    [Range(1000, 9999, ErrorMessage = "Account number must be 4 digits")]
    public int AccountNumber { get; set; }
    
    [Required]
    // [StringLength(1)]
    // [RegularExpression("^[CS]$", ErrorMessage = "AccountType must be C or S")]
    [JsonConverter(typeof(AccountTypeStringToAccountTypeEnumConverter))]
    [Display(Name = "Type")]
    public AccountType AccountType { get; set; }
    
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }
    
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Balance { get; set; }
    
    // Set ambiguous navigation property with InverseProperty annotation or Fluent-API in the McbaContext.cs file.
    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }
    
    
}