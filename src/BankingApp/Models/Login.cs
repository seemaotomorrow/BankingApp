using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankingApp.Models;

public class Login
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Required]
    [StringLength(8)]
    // To do: Data annotation: 8 digits
    public string LoginID { get; set; }

    [Required]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }
    
    [Required]
    [StringLength(94)]
    [Column(TypeName = "char(94)")] //Defines the column type and length in the database
    public string PasswordHash { get; set; }
}