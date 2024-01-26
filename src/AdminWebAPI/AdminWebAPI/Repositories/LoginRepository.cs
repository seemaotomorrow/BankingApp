using AdminWebAPI.Data;
using AdminWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdminWebAPI.Repositories;


public interface ILoginRepository
{
    Task<Login> GetByLoginIDAsync(string loginId);
    Task CreateAsync(Login login);
    Task UpdatePasswordAsync(string loginId, string newPasswordHash);
    Task<Login> GetByCustomerIDAsync(int customerId);
    Task DeleteAsync(string loginId);
}

public class LoginRepository : ILoginRepository
{
    private readonly BankingAppContext _context;

    public LoginRepository(BankingAppContext context)
    {
        _context = context;
    }

    public async Task<Login> GetByLoginIDAsync(string loginId)
    {
        return await _context.Logins.FirstOrDefaultAsync(l => l.LoginID == loginId);
    }

    public async Task CreateAsync(Login login)
    {
        _context.Logins.Add(login);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePasswordAsync(string loginId, string newPasswordHash)
    {
        var login = await _context.Logins.FindAsync(loginId);
        if (login != null)
        {
            login.PasswordHash = newPasswordHash;
            _context.Logins.Update(login);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Login> GetByCustomerIDAsync(int customerId)
    {
        return await _context.Logins.FirstOrDefaultAsync(l => l.CustomerID == customerId);
    }

    public async Task DeleteAsync(string loginId)
    {
        var login = await _context.Logins.FindAsync(loginId);
        if (login != null)
        {
            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();
        }
    }
}

