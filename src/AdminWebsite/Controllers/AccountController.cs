using AdminWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace AdminWebsite.Controllers;

public class AccountController : Controller
{
    private readonly HttpClient _client;
    
    public AccountController(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("apiEndpoint");
    }


    public IActionResult Login() => View();

    
  

    [HttpPost]
    public async Task<IActionResult> Login(AdminLoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Return with validation errors
    }
        // Serialize the login model to JSON
        var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

        // Make the API call
        using var response = await _client.PostAsync("/api/Auth/", content);

        if (response.IsSuccessStatusCode)
        {
            // TODO: Process the response, extract the token if needed
            // Redirect to the main admin page
            return RedirectToAction("Index", "Home");
        }
        else
        {
            // Login failed, add a generic error message
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model); // Return with error message
        }
    }
}




