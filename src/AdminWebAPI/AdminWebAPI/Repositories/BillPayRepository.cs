using AdminWebAPI.Models;
using System;
using BankingApp.Services.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminWebAPI.Data;

namespace BankingApp.Services.IRepositories
{
    public interface IBillPayRepository
    {
        Task<IEnumerable<BillPay>> GetAllAsync();
        Task<BillPay> GetByIdAsync(int billPayId);
        Task AddAsync(BillPay billPay);
        Task UpdateAsync(BillPay billPay);
        Task DeleteAsync(int billPayId);
        
    }
}

public class BillPayRepository : IBillPayRepository
{
    private readonly BankingAppContext _context;

    public BillPayRepository(BankingAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BillPay>> GetAllAsync()
    {
        return await _context.BillPays.ToListAsync();
    }

    public async Task<BillPay> GetByIdAsync(int billPayId)
    {
        return await _context.BillPays.FindAsync(billPayId);
    }

    public async Task AddAsync(BillPay billPay)
    {
        _context.BillPays.Add(billPay);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BillPay billPay)
    {
        _context.Entry(billPay).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int billPayId)
    {
        var billPay = await _context.BillPays.FindAsync(billPayId);
        if (billPay != null)
        {
            _context.BillPays.Remove(billPay);
            await _context.SaveChangesAsync();
        }
    }
}
