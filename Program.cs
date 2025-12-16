using HabitGoalTrackerApp.Data;
using HabitGoalTrackerApp.Models;
using HabitGoalTrackerApp.Services.Implementation;
using HabitGoalTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Database configuration:
// - If DATABASE_URL environment variable is set, use SQL Server with that connection string
// - Otherwise, in Production environment, use in-memory database (for quick testing without SQL Server)
// - In Development, use the connection string from appsettings.json
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var useInMemory = false;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // DATABASE_URL explicitly provided - use SQL Server
    Console.WriteLine("Using SQL Server database (DATABASE_URL provided)");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(databaseUrl));
}
else if (builder.Environment.IsProduction())
{
    // Production without DATABASE_URL - use in-memory database
    Console.WriteLine("WARNING: No DATABASE_URL provided. Using in-memory database (data will be lost on restart).");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("HabitGoalTrackerInMemory"));
    useInMemory = true;
}
else
{
    // Development - use connection string from appsettings.json
    var connectionString = builder.Configuration.GetConnectionString("HabitGoalConnection") 
        ?? throw new InvalidOperationException("Connection string 'HabitGoalConnection' not found.");
    Console.WriteLine("Using SQL Server database (from appsettings.json)");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity with custom options (no default UI)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure application cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.AccessDeniedPath = "/auth/access-denied";
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();

// Register email sender service
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Register application services
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<IInsightsService, InsightsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/home/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// Serve dynamically uploaded files (e.g., profile images)
app.UseStaticFiles();

// Serve pre-built static assets with fingerprinting
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Enable attribute routing
app.MapControllers();

// Only map Razor Pages if you still need them for other features
// Remove this line to completely disable Identity Razor Pages
// app.MapRazorPages().WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Check if using in-memory database (doesn't support migrations)
    if (db.Database.IsInMemory())
    {
        // Ensure schema is created for in-memory database
        db.Database.EnsureCreated();
    }
    else
    {
        // Apply pending migrations for SQL Server
        db.Database.Migrate();
    }
}

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }
