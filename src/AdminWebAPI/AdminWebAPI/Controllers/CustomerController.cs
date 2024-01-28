
using AdminWebAPI.Models;
using AdminWebAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdminWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerRepository _customerRepository;

    public CustomerController(CustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    // GET: api/customer
    [HttpGet]
    public async Task<ActionResult<List<CustomerTest>>> GetAll()
    {
        var customers = await _customerRepository.GetAllAsync();
        
        return Ok(customers);
    }

    // GET api/customer/5
    [HttpGet("{id}")]
    public CustomerTest Get(int id)
    {
        return _customerRepository.GetByIdAsync(id);
    }

    // POST api/customer
    [HttpPost]
    public async Task<ActionResult<CustomerTest>> Post([FromBody] CustomerTest customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return CreatedAtAction(nameof(Get), new { id = customer.CustomerID}, customer);
    }

    // PUT api/customer/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] CustomerTest customer)
    {
        if (id != customer.CustomerID)
        {
            return BadRequest();
        }

        var existingCustomer = _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        await _customerRepository.UpdateAsync(customer);
        return NoContent();
    }
    
    [HttpPut("CustomerLock")]
    public async Task<ActionResult> Lock([FromBody] CustomerModel.CustomerLock customer)
    {
        //if token is invalid
        // make validation
       
        await _customerRepository.LockOrUnlockCustomerAsync(customer.CustomerId);
        return Ok();
    }
}

