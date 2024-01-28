using BankingApp.BackgroundServices;
using BankingApp.Data;
using BankingApp.Models;
using BankingApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Hangfire;

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
builder.Services.AddTransient<IBillPayBackgroundService, BillPayBackgroundService>();

// Add framework services.
// builder.Services.AddMvc();


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

// HTTPS Pipeline configuration
app.UseHttpsRedirection();
app.UseStaticFiles(); 

// Hangfire and dashboard to visualise background job
app.UseHangfireDashboard();
app.MapHangfireDashboard();

// Run Every minute
RecurringJob.AddOrUpdate<IBillPayBackgroundService>("ProcessPendingBillPays", 
    x => x.ProcessPendingBillPays(), Cron.MinuteInterval(1));

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



