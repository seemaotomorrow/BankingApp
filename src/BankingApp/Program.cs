using BankingApp.Data;
using BankingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;



internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
        builder.Services.AddDbContext<BankingAppContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(BankingAppContext)));

            // Enable lazy loading.
            options.UseLazyLoadingProxies();
        });

// Store session into Web-Server memory.
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            // Make the session cookie essential.
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddControllersWithViews(); 
        builder.Services.AddScoped<IPasswordHasher<Login>, PasswordHasher<Login>>();
        var app = builder.Build();
        


// Seed Data
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                SeedData.Initialize(services);
            }
            catch(Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred seeding the DB.");
            }
        }

// HTTPS Pipeline configuration
        app.UseHttpsRedirection();
        app.UseStaticFiles(); //This is wwwroot files
        app.UseRouting();
        app.UseAuthorization();
        app.UseSession();

        app.MapDefaultControllerRoute();

        app.Run();
    }
}