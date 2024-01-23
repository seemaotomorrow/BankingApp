using Microsoft.AspNetCore.Mvc;
using BankingApp.Models;
using BankingApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SimpleHashing.Net;
using BankingApp.ViewModels;

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
        
        var editViewModel = new EditViewModel
        {
            Name = customer.Name,
            TFN = customer.TFN,
            Address =customer.Address,
            City = customer.City,
            State = customer.State,
            PostCode = customer.PostCode,
            Mobile = customer.Mobile
        };

        return View(editViewModel);
    }
            
            
    
    
    // POST: MyProfile/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditViewModel model)
    {

        var customer = await _context.Customers.FindAsync(CustomerID);
        if (customer == null)
        {
            return NotFound();
        }
        
        if (!ModelState.IsValid)
            return View(model);

        customer.Name = model.Name;
        customer.TFN = model.TFN;
        customer.Address = model.Address;
        customer.City = model.City;
        customer.State = model.State;
        customer.PostCode = model.PostCode;
        customer.Mobile = model.Mobile;

        try
        {
            _context.Update(customer);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Profile updated successfully.";
        }
        catch (DbUpdateException)
        {
            // Log the error
            ModelState.AddModelError("", "Unable to save changes. " +
                                         "Try again, and if the problem persists, " +
                                         "see your system administrator.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
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
        
        if (!ModelState.IsValid)
            return View();

        var login = await _context.Logins.FirstOrDefaultAsync(l => l.CustomerID == CustomerID);
        if (login == null)
        {
            return NotFound();
        }
        // This is user to input old PW to compare the logedin one
        var inputOldPCorrect = s_simpleHash.Verify(model.OldPassword, login.PasswordHash);

        
        if(!inputOldPCorrect)
        {
            ModelState.AddModelError("OldPassword", "The current password is incorrect.");
            return View(model);
        }
        
        //need to implement more logic here if user put null. 

        login.PasswordHash = s_simpleHash.Compute(model.NewPassword);
        _context.Update(login);
        await _context.SaveChangesAsync();

        //  adding a success message 
        TempData["SuccessMessage"] = "Password changed successfully.";

        return RedirectToAction(nameof(Index));
    }

  
}


    


