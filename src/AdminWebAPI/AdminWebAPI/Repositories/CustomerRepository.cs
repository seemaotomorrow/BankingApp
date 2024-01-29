using AdminWebAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;


namespace AdminWebAPI.Repositories;

public interface ICustomerRepository<TEntity, TKey> where TEntity: class
{
    
    Task<List<CustomerTest>> GetAllAsync();
    TEntity GetByIdAsync(int customerId);
    Task UpdateAsync(CustomerTest customer);
    Task DeleteAsync(int customerId);
    Task LockOrUnlockCustomerAsync(int customerId);
}

public class CustomerRepository : ICustomerRepository<CustomerTest, int>
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
               var loginDB = await _context.Logins.FirstOrDefaultAsync(x => x.CustomerID == item.CustomerID);
            var customer = new CustomerTest
            {
                CustomerID = item.CustomerID,
                Name = item.Name,
                Address = item.Address,
                City = item.City,
                Mobile = item.Mobile,
                PostCode = item.PostCode,
                State = item.State,
                TFN = item.TFN,
                IsLocked = loginDB.isLocked
                
            };

            customers.Add(customer);
           }
        }
       
      
        return customers;
    }

    public CustomerTest GetByIdAsync(int customerId)
    {
        var item = _context.Customers.Find(customerId);
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

        return customer;
    }

    public async Task UpdateAsync(CustomerTest customer)
    {
        var customerFromDB = _context.Customers.Find(customer.CustomerID);
        
        customerFromDB.Name = customer.Name;
        customerFromDB.Address = customer.Address;
        customerFromDB.TFN = customer.TFN;
        customerFromDB.PostCode = customer.PostCode;
        customerFromDB.Mobile = customer.Mobile;
        customerFromDB.State = customer.State;
        
        _context.Customers.Update(customerFromDB);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task LockOrUnlockCustomerAsync(int customerId)
    {
        var customer = await _context.Logins.Where(x => x.CustomerID == customerId).FirstOrDefaultAsync();
        // IsLoginLocked
        if (customer != null)
        {
            customer.isLocked = !customer.isLocked;
            _context.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}

