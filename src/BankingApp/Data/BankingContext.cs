using Microsoft.EntityFrameworkCore;
using BankingApp.Models;
namespace BankingApp.Data;

public class BankingContext : DbContext
{
    public BankingContext(DbContextOptions<BankingContext> options) : base(options)
    { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    
    public DbSet<BillPay> BillPays { get; set; }
    
    public DbSet<Payee> Payees { get; set; }
    
    
}    