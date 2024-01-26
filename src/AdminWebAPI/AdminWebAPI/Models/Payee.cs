using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebAPI.Models;

public class Payee
{
    public int PayeeID { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string Address { get; set; }

    [Required]
    [StringLength(40)]
    public string City { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 2)]
    public string State { get; set; }

    [Required]
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must be 4 digits")]
    public string Postcode { get; set; }

    [Required]
    [StringLength(14)]
    [RegularExpression(@"^\(0\d\)\s\d{4}\s\d{4}$", ErrorMessage = "Phone must be of the format: (0X) XXXX XXXX")]
    public string Phone { get; set; }
    
}


