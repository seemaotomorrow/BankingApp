using AdminWebAPI.Models;
using AdminWebAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace AdminWebAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionController(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    /// <summary>
    /// Retrieves all transactions.
    /// </summary>
    /// <returns>A list of transactions.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAllTransactions()
    {
        return Ok(await _transactionRepository.GetAllAsync());
    }

    /// <summary>
    /// Retrieves a transaction by its ID.
    /// </summary>
    /// <param name="id">The ID of the transaction.</param>
    /// <returns>The transaction if found; otherwise returns 404 Not Found.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransactionById(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }
        return Ok(transaction);
    }

    /// <summary>
    /// Creates a new transaction.
    /// </summary>
    /// <param name="transaction">The transaction to create.</param>
    /// <returns>A newly created transaction.</returns>
    [HttpPost]
    public async Task<ActionResult<Transaction>> CreateTransaction([FromBody] Transaction transaction)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _transactionRepository.AddAsync(transaction);
        return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.TransactionID }, transaction);
    }

    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    /// <param name="id">The ID of the transaction to update.</param>
    /// <param name="transaction">The updated transaction information.</param>
    /// <returns>An ActionResult.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction transaction)
    {
        if (id != transaction.TransactionID)
        {
            return BadRequest();
        }

        try
        {
            await _transactionRepository.UpdateAsync(transaction);
        }
        catch
        {
            var existingTransaction = await _transactionRepository.GetByIdAsync(id);
            if (existingTransaction == null)
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
    /// Deletes a transaction by its ID.
    /// </summary>
    /// <param name="id">The ID of the transaction to delete.</param>
    /// <returns>An ActionResult.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }
        await _transactionRepository.DeleteAsync(id);
        return NoContent();
    }
}
