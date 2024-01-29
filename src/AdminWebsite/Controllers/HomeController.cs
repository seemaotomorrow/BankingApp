using System.Diagnostics;
using AdminWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdminWebsite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _client;
    
    
    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _client = clientFactory.CreateClient("apiEndpoint");
    }

    // GET: show the login form
    [HttpGet]
    public IActionResult Index()
    {
        // make sure return a new AdminLoginModel
        return View(new AdminLoginModel());
    }

    // POST: login request
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AdminLoginModel model)
    {
        if (!ModelState.IsValid)
        {
            // if does not work ,return to view
            return View(model);
        }
        
        //  JSON
        var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");
        
        // to API sent POST request
        using var response = await _client.PostAsync("/api/Auth/", content);

        if (response.IsSuccessStatusCode)
        {
            //if successed
            

            // back to homepage
            return RedirectToAction("Index", "Customers");
        }
        else
        {
            // if failed, show error
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            // return to view
            return View(model);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}