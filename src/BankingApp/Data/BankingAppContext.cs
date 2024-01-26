using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Data;

public class BankingAppContext : DbContext
{
    public BankingAppContext(DbContextOptions<BankingAppContext> options) : base(options)
    { }
    
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Payee> Payees { get; set; }
    public DbSet<BillPay> BillPays { get; set; }
}