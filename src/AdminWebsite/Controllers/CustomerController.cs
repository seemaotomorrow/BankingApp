using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AdminWebsite.Models; 
using System.Diagnostics;


namespace AdminWebsite.Controllers;

public class CustomersController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<CustomersController> _logger;
    private HttpClient Client => _clientFactory.CreateClient("apiEndpoint");

    public CustomersController(IHttpClientFactory clientFactory,ILogger<CustomersController> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }


    // GET: Customers/Index
    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("/api/Customer/");
        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var customers = JsonConvert.DeserializeObject<List<CustomerDto>>(result);

        return View(customers);
    }

    // GET: Customers/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/customer/{id}");
        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<CustomerDto>(result);

        return View(customer);
    }

    // POST: Customers/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CustomerDto customer)
    {
        if (id != customer.CustomerID)
            return NotFound();

        if (ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await Client.PutAsync($"api/customer/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(customer);
    }
    
    [HttpPost]
    public async Task<IActionResult> LockUnlockCustomer(int customerId)
    {
        // Prepare the request URL and body
        var requestUrl = $"/api/Customer/CustomerLock"; // Adjust the URL as needed
        var jsonInput = new CustomerModel.CustomerLock()
        {
            CustomerId  = customerId

        };
        var requestBody = new StringContent(
            JsonConvert.SerializeObject(jsonInput),
            Encoding.UTF8, "application/json");

        // Send the request to the Web API
      
        var response = await Client.PutAsync(requestUrl,requestBody);

        if (!response.IsSuccessStatusCode) {
            // Log the error for debugging
            _logger.LogError("Error fetching customer data: {StatusCode}", response.StatusCode);
            // Return a user-friendly error message
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        // Redirect back to the list of customers after successful update
        return RedirectToAction("Index");
    }

}





