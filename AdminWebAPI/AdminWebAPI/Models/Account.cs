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

    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }
    
    [NotMapped]
    public decimal MinimumBalanceAllowed => AccountType == AccountType.Saving ? 0.01M : 300M;

    [NotMapped]
    private int FreeTransactions { get; set; } = 2;
    

    public bool HasFreeTransaction()
    {
        var hasFreeTransaction =
            Transactions.Count(x => x.TransactionType is TransactionType.Withdraw or TransactionType.TransferOut) < FreeTransactions;
        return hasFreeTransaction;
    }
    
    public Transaction? ApplyServiceCharge(decimal amount, bool applyServiceCharge)
    {
        // Create a new transaction for the service charge
        if (applyServiceCharge)
            return new Transaction
            {
                AccountNumber = AccountNumber,
                TransactionType = TransactionType.ServiceCharge,
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow
            };
        return null;
    }
}