using Microsoft.AspNetCore.Mvc;

namespace AdminWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        // Authentication logic
        // On success, generate JWT token and return it
    }
}
