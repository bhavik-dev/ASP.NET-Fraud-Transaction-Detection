using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project2Delivery2.DataAccessLayer.Data;
using Project2Delivery2.DataAccessLayer.Models;
using Project2Delivery2.DataAccessLayer.Repositories;
using Project2Delivery2.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add response caching
builder.Services.AddResponseCaching();

// Add memory cache
builder.Services.AddMemoryCache();


// DATABASE CONFIGURATION - Choose ONE of these three options


// OPTION 1: In-Memory Database (Best for testing - no setup required)
// Comment/uncomment the one you want to use
//builder.Services.AddDbContext<FraudDetectionDbContext>(options =>
//    options.UseInMemoryDatabase("frauddetectiondb"));

// OPTION 2: MySQL Database 
// Uncomment this and comment Option 1 to use MySQL

//builder.Services.AddDbContext<FraudDetectionDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("MySqlConnection"),
//        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection"))
//    ));


// OPTION 3: SQL Server Database
// Uncomment this and comment Option 1 to use SQL Server

builder.Services.AddDbContext<FraudDetectionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

// IDENTITY CONFIGURATION
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<FraudDetectionDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IPasswordValidator<ApplicationUser>, CustomPasswordValidator>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IFraudAlertRepository, FraudAlertRepository>();
builder.Services.AddScoped<IMerchantRepository, MerchantRepository>();

var app = builder.Build();


// Seed Database on Startup (Important for first run!)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FraudDetectionDbContext>();

    // For InMemory DB - always recreate
    if (app.Environment.IsDevelopment())
    {
        context.Database.EnsureDeleted(); // Clean slate
        context.Database.EnsureCreated(); // Create and seed
    }
    else
    {
        // For real databases - only create if doesn't exist
        context.Database.EnsureCreated();
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();