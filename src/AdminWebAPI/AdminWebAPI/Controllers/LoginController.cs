using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Repositories;
using AdminWebAPI.Models;


[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILoginRepository _loginRepository;

    public LoginController(ILoginRepository loginRepository)
    {
        _loginRepository = loginRepository;
    }

    /// <summary>
    /// Retrieves login details by login ID.
    /// </summary>
    /// <param name="loginId">The ID of the login to retrieve.</param>
    /// <returns>The login details.</returns>
    [HttpGet("{loginId}")]
    public async Task<ActionResult<Login>> GetLoginById(string loginId)
    {
        var login = await _loginRepository.GetByLoginIDAsync(loginId);
        if (login == null)
        {
            return NotFound();
        }
        return Ok(login);
    }

    /// <summary>
    /// Creates a new login.
    /// </summary>
    /// <param name="login">The login details to create.</param>
    /// <returns>A newly created login.</returns>
    [HttpPost]
    public async Task<ActionResult<Login>> CreateLogin([FromBody] Login login)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _loginRepository.CreateAsync(login);
        return CreatedAtAction(nameof(GetLoginById), new { loginId = login.LoginID }, login);
    }

    /// <summary>
    /// Updates the password hash for an existing login.
    /// </summary>
    /// <param name="loginId">The ID of the login to update.</param>
    /// <param name="passwordHash">The new password hash.</param>
    /// <returns>An ActionResult indicating success or failure.</returns>
    [HttpPut("{loginId}")]
    public async Task<IActionResult> UpdateLogin(string loginId, [FromBody] string passwordHash)
    {
        var login = await _loginRepository.GetByLoginIDAsync(loginId);
        if (login == null)
        {
            return NotFound();
        }

        login.PasswordHash = passwordHash;
        await _loginRepository.UpdatePasswordAsync(loginId, passwordHash);
        return NoContent();
    }

    /// <summary>
    /// Deletes a login by its ID.
    /// </summary>
    /// <param name="loginId">The ID of the login to delete.</param>
    /// <returns>An ActionResult indicating success or failure.</returns>
    [HttpDelete("{loginId}")]
    public async Task<IActionResult> DeleteLogin(string loginId)
    {
        var login = await _loginRepository.GetByLoginIDAsync(loginId);
        if (login == null)
        {
            return NotFound();
        }
        await _loginRepository.DeleteAsync(loginId);
        return NoContent();
    }
}
