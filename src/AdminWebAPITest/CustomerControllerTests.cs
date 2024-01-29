using AdminWebAPI.Controllers;
using AdminWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using AdminWebAPI;
using AdminWebAPI.Data;


public class CustomerControllerTests : IDisposable
{
    private readonly BankingAppContext _context;
    private readonly CustomerRepository _repo;
    public CustomerControllerTests()
    {
        _context = new BankingAppContext(new DbContextOptionsBuilder<BankingAppContext>().
            UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);
    
        // The EnsureCreated method creates the schema based on the current context model.
        _context.Database.EnsureCreated();
        _repo = new CustomerRepository(_context);
        SeedData.Initialize(_context);
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
    
    
    [Fact]
    public async Task GetAll_ReturnsListOfCustomers()
    {
        // Arrange
        var customers = new List<CustomerTest>
        {
            new CustomerTest { CustomerID = 1, Name = "Customer 1" },
            new CustomerTest { CustomerID = 2, Name = "Customer 2" },
            new CustomerTest { CustomerID = 3, Name = "Customer 3" }
        };
        
        

        var controller = new CustomerController(_repo);


        // Act
        var result = await controller.GetAll();

        var cusList = await _context.Customers.ToListAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var model = Assert.IsAssignableFrom<List<CustomerTest>>(okResult.Value);
        Assert.Equal(customers.Count, model.Count);
    }
    
    [Fact]
    public async Task Put_UpdatesCustomer()
    {
        // Arrange
        var customerId = 1;
        var existingCustomerTest = new CustomerTest { CustomerID = customerId, Name = "Existing Customer" };
    
        // Convert CustomerTest to Customer
        var existingCustomer = new Customer
        {
            CustomerID = existingCustomerTest.CustomerID,
            Name = existingCustomerTest.Name
            
        };
    
        _context.Customers.Add(existingCustomer);
        await _context.SaveChangesAsync();

        var updatedCustomerTest = new CustomerTest { CustomerID = customerId, Name = "Updated Customer" };
    
        // Convert CustomerTest to Customer for the update
        var updatedCustomer = new Customer
        {
            CustomerID = updatedCustomerTest.CustomerID,
            Name = updatedCustomerTest.Name
            // other properties as needed
        };
    
        var controller = new CustomerController(_repo);

        // Act
        var result = await controller.Put(customerId, updatedCustomerTest); // Assuming controller.Put expects CustomerTest

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        var customerInDb = await _context.Customers.FindAsync(customerId);
        Assert.NotNull(customerInDb);
        Assert.Equal(updatedCustomer.Name, customerInDb.Name);
    }

    
    
}

















