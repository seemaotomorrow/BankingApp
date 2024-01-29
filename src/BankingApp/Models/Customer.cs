using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankingApp.Models;

public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Range(1000, 9999, ErrorMessage = "Customer ID must be 4 digits")]
    public int CustomerID { get; set; }

    [Required, StringLength(50)] 
    public string Name { get; set; }

    [StringLength(11)]
    [RegularExpression(@"^\d{3}\s\d{3}\s\d{3}$", ErrorMessage = "TFN must be in the format XXX XXX XXX")]
    public string? TFN { get; set; }

    [StringLength(50)] 
    public string? Address { get; set; }

    [StringLength(40)] 
    public string? City { get; set; }

    [StringLength(3, MinimumLength = 2)]
    [RegularExpression(@"^[A-Z]{2,3}$", ErrorMessage = "State must be a 2 or 3 lettered Australian state")]
    // To do: valid Australia state name
    public string? State { get; set; }

    [StringLength(4)]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must be 4 digits")]
    public string? PostCode { get; set; }

    [StringLength(12)]
    [RegularExpression(@"04\d{2}\s\d{3}\s\d{3}$", ErrorMessage = "Mobile number must be of the format: 04XX XXX XXX")]
    public string? Mobile { get; set; }
    
    public virtual List<Account> Accounts { get; set; }

    [NotMapped]
    public Login Login { get; set; }

}