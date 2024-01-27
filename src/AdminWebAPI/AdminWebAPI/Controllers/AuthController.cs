using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdminWebAPI.Models;
using AdminWebAPI.Services;


namespace AdminWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // private readonly JwtSettings _jwtSettings;

        // public AuthController(JwtSettings jwtSettings)
        // {
        //     _jwtSettings = jwtSettings;
        // }

        public AuthController(){

        }

        // [HttpPost("login")]
         [HttpPost]
        public IActionResult Login([FromBody] AdminLogin login)
        {
            // 
            if (login.Username != "admin" || login.Password != "admin")
            {
                return Unauthorized();
            }

            // 
            // var token = GenerateJwtToken();
             var token = "ssda";

            return Ok(new { Token = token });
        }

        // private string GenerateJwtToken()
        // {
        //     var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        //     var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //     var tokenDescriptor = new SecurityTokenDescriptor
        //     {
        //         Subject = new ClaimsIdentity(new[] 
        //         {
        //             new Claim("sub", "admin") // subject claim
        //             //could add more
        //         }),
        //         Expires = DateTime.UtcNow.AddHours(1),
        //         SigningCredentials = credentials
        //     };

        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var token = tokenHandler.CreateToken(tokenDescriptor);

        //     return tokenHandler.WriteToken(token);
        // }
    }
}