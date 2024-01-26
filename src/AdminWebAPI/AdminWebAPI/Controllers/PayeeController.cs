using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Models;
using AdminWebAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PayeeController : ControllerBase
{
    private readonly IPayeeRepository _payeeRepository;

    public PayeeController(IPayeeRepository payeeRepository)
    {
        _payeeRepository = payeeRepository;
    }

    /// <summary>
    /// Retrieves all payees.
    /// </summary>
    /// <returns>A list of payees.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payee>>> GetAllPayees()
    {
        return Ok(await _payeeRepository.GetAllAsync());
    }

    /// <summary>
    /// Retrieves a specific payee by ID.
    /// </summary>
    /// <param name="id">The ID of the payee to retrieve.</param>
    /// <returns>The payee details if found; otherwise returns 404 Not Found.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Payee>> GetPayee(int id)
    {
        var payee = await _payeeRepository.GetByIdAsync(id);
        if (payee == null)
        {
            return NotFound();
        }
        return Ok(payee);
    }

    /// <summary>
    /// Creates a new payee.
    /// </summary>
    /// <param name="payee">The payee to create.</param>
    /// <returns>A newly created payee.</returns>
    [HttpPost]
    public async Task<ActionResult<Payee>> CreatePayee([FromBody] Payee payee)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _payeeRepository.AddAsync(payee);
        return CreatedAtAction(nameof(GetPayee), new { id = payee.PayeeID }, payee);
    }

    /// <summary>
    /// Updates a specific payee.
    /// </summary>
    /// <param name="id">The ID of the payee to update.</param>
    /// <param name="payee">The updated payee information.</param>
    /// <returns>An ActionResult indicating success or failure.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePayee(int id, [FromBody] Payee payee)
    {
        if (id != payee.PayeeID)
        {
            return BadRequest();
        }

        try
        {
            await _payeeRepository.UpdateAsync(payee);
        }
        catch
        {
            if (!await PayeeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific payee by ID.
    /// </summary>
    /// <param name="id">The ID of the payee to delete.</param>
    /// <returns>An ActionResult indicating success or failure.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayee(int id)
    {
        var payee = await _payeeRepository.GetByIdAsync(id);
        if (payee == null)
        {
            return NotFound();
        }
        await _payeeRepository.DeleteAsync(id);
        return NoContent();
    }

    private async Task<bool> PayeeExists(int id)
    {
        return await _payeeRepository.GetByIdAsync(id) != null;
    }
}
