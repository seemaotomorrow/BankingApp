using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using BankingApp.Models;

namespace BankingApp.Data;

public class CustomerPreloadService
{
    private readonly BankingContext _context;
    private readonly HttpClient _httpClient;
    private readonly string _customerServiceUrl = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

    public CustomerPreloadService(BankingContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task PreloadCustomerDataAsync()
    {
        if (_context.Customers.Any())
        {
            return; // had data
        }

        var response = await _httpClient.GetAsync(_customerServiceUrl);
        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync();

        var customers = JsonSerializer.Deserialize<List<Customer>>(jsonString);

        if (customers != null)
        {
            foreach (var customer in customers)
            {
                // 这里您可以添加其他逻辑，如验证预加载的客户数据
                
                if (IsValidCustomer(customer))
                {
                    _context.Customers.Add(customer);
                }
            }
            await _context.SaveChangesAsync();
        }
    }

    private bool IsValidCustomer(Customer customer)
    {
       
        return true;
    }
}

