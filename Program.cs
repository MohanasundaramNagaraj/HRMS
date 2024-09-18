using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;

using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Utilities;
using SparkHRMS.Interfaces;
using SparkHRMS.Repositories;
using Hangfire;
using Hangfire.SqlServer;
//using ElmahCore.Mvc;
using SparkHRMS.Filters;
using SparkHRMS.Services;
using Newtonsoft.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddMvc();

// Database initializer
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
//builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Authentication and authorization
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(/*options => options.SignIn.RequireConfirmedAccount = true*/)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddDefaultTokenProviders();
//.AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>();

//Services configuration
builder.Services.AddScoped<ApplicationUser>();
builder.Services.AddTransient<SparkHRMS.Interfaces.IEmailSender,  EmailSender>();

// Session cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});



string hangfireConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseFilter(new AutomaticRetryAttribute { Attempts = 0 }) // Retry attempts filter added by vigneshwaran 
    .UseStorage(
        new SqlServerStorage(
            hangfireConnectionString,

            new SqlServerStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(10),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true,
                DashboardJobListLimit = 25000,
                TransactionTimeout = TimeSpan.FromMinutes(1),
                SchemaName = "HRHangfire",
            }
)
    ));


//// Add the processing server as IHostedService
//builder.Services.AddHangfireServer(options => options.WorkerCount = Environment.ProcessorCount * 5);

builder.Services.AddSignalR(options =>
options.EnableDetailedErrors = true);

// Getting rid of code that mapped one object to another.
////builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddApiVersioning();

builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
builder.Services.AddControllersWithViews();

services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Invoke function to seed database
SeedDatabase();

app.UseAuthentication();
app.UseAuthorization();

//app.UseElmah();

app.UseHangfireDashboard();
app.UseHangfireServer();

var configuration = app.Services.GetRequiredService<IConfiguration>();
var scheduler = new EmailScheduler(configuration);
scheduler.ScheduleEmailJob();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Redirect root URL to the login page
    endpoints.MapGet("/", context =>
    {
        context.Response.Redirect("/Identity/Account/Login");
        return Task.CompletedTask;
    });
    endpoints.MapRazorPages();

    endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter(app.Services) },
        IgnoreAntiforgeryToken = true
    });

});

app.MapControllerRoute(
    name: "default",
     pattern: "{controller=Home}/{action=Index}/{id?}");

//app.UseSwagger();

app.UseHttpsRedirection();

app.Run();

void SeedDatabase()
{
    using var scope = app.Services.CreateAsyncScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    initializer.Initialize();
}
