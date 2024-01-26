using AdminWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebsite.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(AdminLoginModel model)
    {
        if (model.Username == "admin" && model.Password == "admin")
        {
            
            // Redirect to the main admin page
            return RedirectToAction("Index", "Home");
        }

        return View(model); // Return with error message
    }
}



