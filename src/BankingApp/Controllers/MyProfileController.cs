using Microsoft.AspNetCore.Mvc;
using BankingApp.Models;
using BankingApp.Data;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SimpleHashing.Net;

namespace BankingApp.Controllers;

public class MyProfileController: Controller
{
    private static readonly ISimpleHash s_simpleHash = new SimpleHash();
    private readonly BankingAppContext _context;
    private readonly IPasswordHasher<Login> _passwordHasher; // Use the interface for dependency injection
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
    
    // Constructor
    public MyProfileController(BankingAppContext context, IPasswordHasher<Login> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher; // Store the injected password hasher
    }
    
    // Get MyProfile
    public async Task<IActionResult> Index()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);
        if (customer == null)
        {
            return NotFound();
        }
        return View("Index", customer);
    }
    
    // GET: MyProfile/Edit
    public async Task<IActionResult> Edit()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);
        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }
    
    // POST: MyProfile/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Customer customer)
    {
        if (customer.CustomerID != CustomerID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.Customers.Any(e => e.CustomerID == customer.CustomerID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        return View(customer);
    }

    // GET: MyProfile/ChangePassword
    public IActionResult ChangePassword()
    {
        var viewModel = new ChangePasswordViewModel { CustomerID = CustomerID };
        return View(viewModel);
    }
    
    // POST: MyProfile/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (model.CustomerID != CustomerID)
        {
            return NotFound();
        }

        var login = await _context.Logins.FirstOrDefaultAsync(l => l.CustomerID == CustomerID);
        if (login == null)
        {
            return NotFound();
        }
        
        // Check all the validations
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        // Check if User input correct old password
        var inputOldPCorrect = s_simpleHash.Verify(model.OldPassword, login.PasswordHash);
        if(!inputOldPCorrect)
        {
            ModelState.AddModelError("OldPassword", "The current password is incorrect.");
            return View(model);
        }

        login.PasswordHash = s_simpleHash.Compute(model.NewPassword);
        _context.Update(login);
        await _context.SaveChangesAsync();

        //  adding a success message 
        TempData["SuccessMessage"] = "Password changed successfully.";

        return RedirectToAction(nameof(Index));
    }
}


    //should be able to update the profile fields without changing the PW
    //should be able to change the password without updating the profile
    


