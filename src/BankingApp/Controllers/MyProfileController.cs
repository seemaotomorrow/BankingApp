using Microsoft.AspNetCore.Mvc;
using BankingApp.Models;
using BankingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Controllers;

public class MyProfileController: Controller
{
    private readonly BankingContext _context; //may need to replace real DB  maybe!
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
    

    public MyProfileController(BankingContext context)
    {
        _context = context;
    }
    
    // get MyProfile
    public async Task<IActionResult> Index()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        if (customer == null)
        {
            return NotFound();
        }

        return View("MyProfile",customer);
    }
    
    //Edit MyProfile
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
            catch (DbUpdateConcurrencyException)
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

    //Lea: have to implement the ChangePassword method here
    //The user should be able to change their password INDEPENDENTLY from the customer infor.
    //also any update must be written back to the databse
    //should be able to update the profile fields without changing the PW
    //should be able to change the password without updating the profile
}

