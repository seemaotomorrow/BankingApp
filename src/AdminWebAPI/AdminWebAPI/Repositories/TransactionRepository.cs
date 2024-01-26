using AdminWebAPI.Data;
using AdminWebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AdminWebAPI.Repositories;


public interface ITransactionRepository
{
Task<IEnumerable<Transaction>> GetAllAsync();
Task<Transaction> GetByIdAsync(int transactionId);
Task AddAsync(Transaction transaction);
Task UpdateAsync(Transaction transaction);
Task<IEnumerable<Transaction>> GetByAccountNumberAsync(int accountNumber);
Task DeleteAsync(int transactionId);

}


public class TransactionRepository : ITransactionRepository
{
    private readonly BankingAppContext _context;

    public TransactionRepository(BankingAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<Transaction> GetByIdAsync(int transactionId)
    {
        return await _context.Transactions.FindAsync(transactionId);
    }

    public async Task AddAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByAccountNumberAsync(int accountNumber)
    {
        return await _context.Transactions
            .Where(t => t.AccountNumber == accountNumber)
            .ToListAsync();
    }
    
    public async Task UpdateAsync(Transaction transaction)
    {
        // Check if the entity exists in the context
        var existingTransaction = await _context.Transactions.FindAsync(transaction.TransactionID);
        if (existingTransaction != null)
        {
            // Update the existing entity with the new values
            _context.Entry(existingTransaction).CurrentValues.SetValues(transaction);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Handle the case when the transaction does not exist
            throw new KeyNotFoundException("Transaction not found.");
        }
    }

    public async Task DeleteAsync(int transactionId)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}

