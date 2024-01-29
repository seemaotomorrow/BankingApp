
using AdminWebAPI.Models;
using Newtonsoft.Json;
using AdminWebAPI.Utilities;

namespace AdminWebAPI.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<BankingAppContext>();
            
        Initialize(context);
    }

    public static void Initialize(BankingAppContext context)
    {
        //var context = serviceProvider.GetRequiredService<BankingAppContext>();


        // Look for customers.
        if (context.Customers.Any())
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

        context.SaveChanges();
    }
}
