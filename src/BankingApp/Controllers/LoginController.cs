using Microsoft.AspNetCore.Mvc;
using BankingApp.Data; 
using BankingApp.Models; 
using System.Threading.Tasks;
using SimpleHashing.Net;  

namespace BankingApp.Controllers;

[Route("/Banking/SecureLogin")]
public class LoginController : Controller
{
    private readonly BankingContext _context; 

    public LoginController(BankingContext context)
    {
        _context = context;
    }

    // GET page
    [HttpGet]
    public IActionResult Login() => View();

    // POST 
    [HttpPost]
    public async Task<IActionResult> Login(int loginID, string password)
    {
        var login = await _context.Logins.FindAsync(loginID);
        if (login == null || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(new Login { LoginID = loginID });
        }

        // use SimpleHashing.Net to verify pw
        if (!new SimpleHash().Verify(password, login.PasswordHash))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(new Login { LoginID = loginID });
        }

        
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "Customer"); 
    }

    [Route("LogoutNow")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home"); // back to homepage
    }
}