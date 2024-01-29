using BankingApp.Data;
using BankingApp.Models;

namespace BankingApp.Repositories;


public interface IAccountRepository
{
    public Account? GetAccount(int accountNumber);
    public void UpdateAccount(Account account);

}
public class AccountRepository : IAccountRepository
{
    private readonly BankingAppContext _context;

    public AccountRepository(BankingAppContext context)
    {
        _context = context;
    }

    public Account? GetAccount(int accountNumber)
    {
        return _context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
    }

    public void UpdateAccount(Account account)
    {
        _context.Accounts.Update(account);
        _context.SaveChanges();
    }
}