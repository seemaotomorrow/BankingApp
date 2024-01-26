using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AdminWebsite.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminWebsite.Controllers;

public class CustomersController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public CustomersController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    // GET: Customers/Index
    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("api/customers");
        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var customers = JsonConvert.DeserializeObject<List<CustomerDto>>(result);

        return View(customers);
    }

    // GET: Customers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Customers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerDto customer)
    {
        if (ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await Client.PostAsync("api/customers", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(customer);
    }

    // GET: Customers/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/customers/{id}");
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
            var response = await Client.PutAsync($"api/customers/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(customer);
    }

    // GET: Customers/Delete/1
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/customers/{id}");
        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<CustomerDto>(result);

        return View(customer);
    }

    // POST: Customers/Delete/1
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await Client.DeleteAsync($"api/customers/{id}");
        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return NotFound();
    }
    
    
    [HttpPost]
    public async Task<IActionResult> LockUnlockCustomer(int customerId, bool lockAccount)
    {
        // Prepare the request URL and body
        var requestUrl = $"api/customers/{customerId}/lockstatus"; // Adjust the URL as needed
        var requestBody = new StringContent(
            JsonConvert.SerializeObject(new { IsLocked = lockAccount }),
            Encoding.UTF8, "application/json");

        // Send the request to the Web API
        var response = await Client.PutAsync(requestUrl, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            // Handle error response from the API
            // Log the error, display a message, etc.
            return View("Error"); 
        }

        // Redirect back to the list of customers after successful update
        return RedirectToAction("Index");
    }

}


//Note for lea: Ensure your WebAPI has the necessary endpoint to handle lock/unlock requests
// especially around authentication and authorization to ensure that only authorized users can lock or unlock customer accounts.


