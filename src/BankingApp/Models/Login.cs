using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankingApp.Models;

public class Login
{
    [Key] //PK?
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LoginID { get; set; }

    [Required]
    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    
    //Navigation property to the Customer entity
    public virtual Customer Customer { get; set; }
    
    [Required]
    [StringLength(94)]
    [Column(TypeName = "char(94)")] //Defines the column type and length in the database
    public string PasswordHash { get; set; }
    

    
}