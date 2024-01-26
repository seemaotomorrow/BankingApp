using AdminWebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminWebAPI.Models;
using AdminWebAPI.Data;
using Microsoft.EntityFrameworkCore;
namespace AdminWebAPI.Repositories;

public interface IPayeeRepository
{
Task<IEnumerable<Payee>> GetAllAsync();
Task<Payee> GetByIdAsync(int payeeId);
Task AddAsync(Payee payee);
Task UpdateAsync(Payee payee);
Task DeleteAsync(int payeeId);
}


public class PayeeRepository : IPayeeRepository
{
    private readonly BankingAppContext _context;

    public PayeeRepository(BankingAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Payee>> GetAllAsync()
    {
        return await _context.Payees.ToListAsync();
    }

    public async Task<Payee> GetByIdAsync(int payeeId)
    {
        return await _context.Payees.FindAsync(payeeId);
    }

    public async Task AddAsync(Payee payee)
    {
        _context.Payees.Add(payee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Payee payee)
    {
        _context.Entry(payee).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int payeeId)
    {
        var payee = await _context.Payees.FindAsync(payeeId);
        if (payee != null)
        {
            _context.Payees.Remove(payee);
            await _context.SaveChangesAsync();
        }
    }
}



