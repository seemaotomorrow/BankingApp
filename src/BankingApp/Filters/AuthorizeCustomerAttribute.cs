using BankingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

// Reference: RMIT WDT Day6-Lab McbaExampleWithLogin
namespace BankingApp.Filters;

public class AuthorizeCustomerAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
        if(!customerID.HasValue)
            context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}
