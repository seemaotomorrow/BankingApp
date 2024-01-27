using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankingApp.Tools.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankingApp.ViewModels;

public class CreateBillPayViewModel
{
    // For display
    [ValidateNever]
    public SelectList AccountNumbers { get; set; }
    [ValidateNever]
    public SelectList PayeeIDs { get; set; }
    
    // For get user input
    [Column(TypeName = "money")]
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value")]
    public decimal Amount { get; set; }
    
    [Required]
    public int SelectedAccountNumber { get; set; }
    
    [Required]
    public int SelectedPayeeID { get; set; }
    
    [Required]
    [Column(TypeName = "datetime2")]
    [ValidateDateUtilities(ErrorMessage = "Scheduled date has to be future")]
    public DateTime ScheduleTimeUtc { get; set; }

    [Required(ErrorMessage = "Period is required")]
    [RegularExpression("^[OM]$", ErrorMessage = "Period must be 'O' or 'M'")]
    public char Period { get; set; } //  (One-off 'O' or Monthly 'M')
}