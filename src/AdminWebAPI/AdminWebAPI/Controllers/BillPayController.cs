using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Models;
using AdminWebAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.Services.IRepositories;

[ApiController]
[Route("api/[controller]")]
public class BillPayController : ControllerBase
{
    private readonly IBillPayRepository _billPayRepository;

    public BillPayController(IBillPayRepository billPayRepository)
    {
        _billPayRepository = billPayRepository;
    }

    /// <summary>
    /// Retrieves all bill payments.
    /// </summary>
    /// <returns>A list of all bill payments.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BillPay>>> GetAllBillPays()
    {
        return Ok(await _billPayRepository.GetAllAsync());
    }

    /// <summary>
    /// Retrieves a specific bill payment by its ID.
    /// </summary>
    /// <param name="id">The ID of the bill payment to retrieve.</param>
    /// <returns>The bill payment details.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<BillPay>> GetBillPay(int id)
    {
        var billPay = await _billPayRepository.GetByIdAsync(id);
        if (billPay == null)
        {
            return NotFound();
        }
        return Ok(billPay);
    }

    /// <summary>
    /// Creates a new bill payment.
    /// </summary>
    /// <param name="billPay">The bill payment to create.</param>
    /// <returns>A newly created bill payment.</returns>
    [HttpPost]
    public async Task<ActionResult<BillPay>> CreateBillPay([FromBody] BillPay billPay)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _billPayRepository.AddAsync(billPay);
        return CreatedAtAction(nameof(GetBillPay), new { id = billPay.BillPayID }, billPay);
    }

    /// <summary>
    /// Updates a specific bill payment.
    /// </summary>
    /// <param name="id">The ID of the bill payment to update.</param>
    /// <param name="billPay">The updated bill payment.</param>
    /// <returns>An ActionResult indicating success or failure.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBillPay(int id, [FromBody] BillPay billPay)
    {
        if (id != billPay.BillPayID)
        {
            return BadRequest();
        }

        try
        {
            await _billPayRepository.UpdateAsync(billPay);
        }
        catch
        {
            if (!await BillPayExists(id))
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
    /// Deletes a specific bill payment by its ID.
    /// </summary>
    /// <param name="id">The ID of the bill payment to delete.</param>
    /// <returns>An ActionResult indicating success or failure.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBillPay(int id)
    {
        var billPay = await _billPayRepository.GetByIdAsync(id);
        if (billPay == null)
        {
            return NotFound();
        }

        await _billPayRepository.DeleteAsync(id);
        return NoContent();
    }

    private async Task<bool> BillPayExists(int id)
    {
        return await _billPayRepository.GetByIdAsync(id) != null;
    }
}
