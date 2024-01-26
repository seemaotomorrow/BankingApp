using BankingApp.Data;
using BankingApp.Models;
using BankingApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

////////////// Hangfire Config ///////////////
// Add Hangfire services.
// Where hangfire store all the info
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString(nameof(BankingAppContext))));
// Add the processing server as IHostedService
builder.Services.AddHangfireServer();
// Add framework services.
builder.Services.AddMvc();


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

builder.Services.AddScoped<IPasswordHasher<Login>, PasswordHasher<Login>>();
builder.Services.AddScoped<IBillPayRepository, BillPayRepository>();
builder.Services.AddControllersWithViews(); 

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

// Hangfire Configuration
// HTTPS Pipeline configuration
app.UseHttpsRedirection();
app.UseStaticFiles(); //This is wwwroot files

app.UseHangfireDashboard();
app.UseHangfireServer();

app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapDefaultControllerRoute();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
});
app.Run(); 



