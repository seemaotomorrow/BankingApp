using BankingApp.Data;
using Microsoft.AspNetCore.Mvc;
using BankingApp.Models;
using SimpleHashing.Net;

namespace BankingApp.Controllers;

[Route("/BankingApp/SecureLogin")]
public class LoginController: Controller
{
    // Naming convention: static have s, private have 
    private static readonly ISimpleHash s_simpleHash = new SimpleHash();

    private readonly BankingAppContext _context;

    public LoginController(BankingAppContext context)

    {
        _context = context;
    }
    
    public IActionResult Login() => View();
    
    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        var login = await _context.Logins.FindAsync(loginID);
        // Check all the conditions
        if(login == null || string.IsNullOrEmpty(password) || !s_simpleHash.Verify(password, login.PasswordHash))
        { 
            // Customize error message
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(new Login { LoginID = loginID });
        }

        if (login.isLocked)
        {
            // Customize error message
            ModelState.AddModelError("LoginFailed", "LoginFailed, Your account is locked");
            return View(new Login { LoginID = loginID });
        }

        login.Customer = await _context.Customers.FindAsync(login.CustomerID);

        // Login customer.
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "Customer");
    }
    
    // Logout
    [Route("LogoutNow")]
    public IActionResult Logout()
    {
        // Logout customer.
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}