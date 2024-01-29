using BankingApp.Data;
using BankingApp.Models;

namespace BankingApp.Repositories;

public interface ITransactionRepository
{
    void AddTransaction(Transaction transaction);
    IEnumerable<Transaction> GetTransactionsForAccount(int accountNumber);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly BankingAppContext _context;

    public TransactionRepository(BankingAppContext context)
    {
        _context = context;
    }

    public void AddTransaction(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        _context.SaveChanges();
    }

    public IEnumerable<Transaction> GetTransactionsForAccount(int accountNumber)
    {
        return _context.Transactions.Where(t => t.AccountNumber == accountNumber).ToList();
    }
}