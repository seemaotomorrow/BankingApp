using AdminWebAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;


namespace AdminWebAPI.Repositories;

public interface ICustomerRepository
{
    Task<List<CustomerTest>> GetAllAsync();
    Task<Customer> GetByIdAsync(int customerId);
    // Task AddAsync(CustomerTest customer);
    // Task UpdateAsync(CustomerTest customer);
    Task DeleteAsync(int customerId);
    Task LockOrUnlockCustomerAsync(int customerId, bool isLocked);
   
}

public class CustomerRepository : ICustomerRepository
{
    private readonly BankingAppContext _context;

    public CustomerRepository(BankingAppContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerTest>> GetAllAsync()
    {
        var customers = new List<CustomerTest>();

        var customerDB = await _context.Customers.ToListAsync();
        if(customerDB !=null)
        {
           foreach (var item in customerDB)
           {
            var customer = new CustomerTest
            {
                CustomerID = item.CustomerID,
                Name = item.Name,
                Address = item.Address,
                City = item.City,
                Mobile = item.Mobile,
                PostCode = item.PostCode,
                State = item.State,
                TFN = item.TFN
            };

            customers.Add(customer);
           }
        }
       
      
        return customers;
    }

    public async Task<Customer> GetByIdAsync(int customerId)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerID == customerId);
    }

    // public async Task AddAsync(CustomerTest customer)
    // {
    //     _context.Customers.Add(customer);
    //     await _context.SaveChangesAsync();
    // }

    // public async Task UpdateAsync(CustomerTest customer)
    // {
    //     _context.Customers.Update(customer);
    //     await _context.SaveChangesAsync();
    // }

    public async Task DeleteAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task LockOrUnlockCustomerAsync(int customerId, bool isLocked)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            // customer.IsLoginLocked = isLocked;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}

