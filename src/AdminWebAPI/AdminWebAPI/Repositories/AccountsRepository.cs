using System.Collections.Generic;
using System.Threading.Tasks;
using AdminWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AdminWebAPI.Data;

namespace AdminWebAPI.Repositories;



public interface IAccountRepository
{
Task<IEnumerable<Account>> GetAllAsync();
Task<Account> GetByAccountNumberAsync(int accountNumber);
Task AddAsync(Account account);
Task UpdateAsync(Account account);
Task DeleteAsync(int accountNumber);
Task<IEnumerable<Account>> GetAccountsByCustomerIDAsync(int customerId);

}




public class AccountRepository : IAccountRepository
{
    private readonly BankingAppContext _context;

    public AccountRepository(BankingAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        return await _context.Accounts.ToListAsync();
    }

    public async Task<Account> GetByAccountNumberAsync(int accountNumber)
    {
        return await _context.Accounts.FindAsync(accountNumber);
    }

    public async Task AddAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account)
    {
        _context.Entry(account).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int accountNumber)
    {
        var account = await _context.Accounts.FindAsync(accountNumber);
        if (account != null)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Account>> GetAccountsByCustomerIDAsync(int customerId)
    {
        return await _context.Accounts
            .Where(a => a.CustomerID == customerId)
            .ToListAsync();
    }
}



  

