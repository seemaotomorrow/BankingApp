using BankingApp.Models;
using BankingApp.Utilities;
using Newtonsoft.Json;

namespace BankingApp.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<BankingAppContext>();
        
        // Look for customers.
        if(context.Customers.Any())
            return; // DB has already been seeded.
        
        // To do: Seed data from JSON file
        const string customerUrl = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";
        
        using var client = new HttpClient();
        var json = client.GetStringAsync(customerUrl).Result;

        // Deserialize the JSON into a list of Customer objects
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json, new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new AccountTypeStringToAccountTypeEnumConverter()
            },
            DateFormatString = "dd/MM/yyyy hh:mm:ss tt" 
        });

        foreach (var customer in customers)
        {
            context.Customers.Add(customer);

            var login = customer.Login;
            login.CustomerID = customer.CustomerID;
            context.Logins.Add(login);
            
            foreach (var account in customer.Accounts)
            {
                var transactions = account.Transactions;
                
                foreach (var transaction in transactions)
                {
                    transaction.TransactionType = TransactionType.Deposit;
                    context.Transactions.Add(transaction);
                }

                var initialBalance = transactions.Sum(x => x.Amount);
                account.Balance = initialBalance;
                account.CustomerID = customer.CustomerID;
                context.Accounts.Add(account);
            }
        }
        
        // Hard code Payees data
        context.Payees.AddRange(
            new Payee
            {
                Name = "Water Company",
                Address = "123 Main Street",
                City = "Sydney",
                Phone = "0434567890",
                Postcode = "2000",
                State = "NSW"
            },
            new Payee
            {
                Name = "Electric Company",
                Address = "456 Elm Street",
                City = "Melbourne",
                Phone = "0987654321",
                Postcode = "3000",
                State = "VIC"
            },
            new Payee
            {
                Name = "Telstra",
                Address = "789 Maple Avenue",
                City = "Brisbane",
                Phone = "0456789123",
                Postcode = "4000",
                State = "QLD"
            },
            new Payee
            {
                Name = "Origin Hot Water",
                Address = "101 Oak Street",
                City = "Perth",
                Phone = "0865432190",
                Postcode = "6000",
                State = "WA"
            });
        
        context.SaveChanges();
    }
}
