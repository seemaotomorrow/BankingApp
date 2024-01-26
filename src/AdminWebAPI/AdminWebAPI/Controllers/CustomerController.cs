
using AdminWebAPI.Repositories;
using AdminWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    // GET: api/customer
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
    {
        var customers = await _customerRepository.GetAllAsync();
        return Ok(customers);
    }

    // GET api/customer/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> Get(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    // POST api/customer
    [HttpPost]
    public async Task<ActionResult<Customer>> Post([FromBody] Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _customerRepository.AddAsync(customer);
        return CreatedAtAction(nameof(Get), new { id = customer.CustomerID}, customer);
    }

    // PUT api/customer/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] Customer customer)
    {
        if (id != customer.CustomerID)
        {
            return BadRequest();
        }

        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        await _customerRepository.UpdateAsync(customer);
        return NoContent();
    }

    // DELETE api/customer/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        await _customerRepository.DeleteAsync(id);
        return NoContent();
    }
}

