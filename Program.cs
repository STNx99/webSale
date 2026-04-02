using WebsiteBanHang.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using WebsiteBanHang.DataAccess;
using Microsoft.AspNetCore.Identity;
using WebsiteBanHang.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mysqlConnectionString = BuildMySqlConnectionString();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        mysqlConnectionString,
        new MySqlServerVersion(new Version(8, 0, 36)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireModeratorRole", policy => policy.RequireRole("Admin", "Moderator"));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IProductRepository, MockProductRepository>();
builder.Services.AddScoped<ICategoryRepository, MockCategoryRepository>();

// Add our seed service for roles and users
builder.Services.AddScoped<DbInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

int? httpsPort = builder.Configuration.GetValue<int?>("HTTPS_PORT")
    ?? builder.Configuration.GetValue<int?>("ASPNETCORE_HTTPS_PORT");

if (!httpsPort.HasValue && int.TryParse(Environment.GetEnvironmentVariable("HTTPS_PORT"), out var configuredHttpsPort))
{
    httpsPort = configuredHttpsPort;
}

if (!httpsPort.HasValue && int.TryParse(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT"), out var aspNetHttpsPort))
{
    httpsPort = aspNetHttpsPort;
}

if (httpsPort.HasValue)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed the database with roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbInitializer = services.GetRequiredService<DbInitializer>();
    dbInitializer.Initialize().Wait();
}

app.Run();

string? BuildMySqlConnectionString()
{
    var mysqlUrl = Environment.GetEnvironmentVariable("MYSQL_URL")
        ?? Environment.GetEnvironmentVariable("MYSQL_PUBLIC_URL");

    if (!string.IsNullOrWhiteSpace(mysqlUrl) && Uri.TryCreate(mysqlUrl, UriKind.Absolute, out var mysqlUri))
    {
        var userInfo = mysqlUri.UserInfo.Split(':', 2);
        if (userInfo.Length == 2)
        {
            var urlUser = Uri.UnescapeDataString(userInfo[0]);
            var urlPassword = Uri.UnescapeDataString(userInfo[1]);
            var urlDatabase = mysqlUri.AbsolutePath.Trim('/');
            var urlHost = mysqlUri.Host;
            var urlPort = mysqlUri.Port > 0 ? mysqlUri.Port : 3306;
            var sslMode = IsLocalHost(urlHost) ? "None" : "Required";

            return $"Server={urlHost};Port={urlPort};Database={urlDatabase};User={urlUser};Password={urlPassword};SslMode={sslMode};AllowPublicKeyRetrieval=True;";
        }
    }

    var mysqlHost = Environment.GetEnvironmentVariable("MYSQLHOST");
    var mysqlPort = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";
    var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQLDATABASE")
        ?? Environment.GetEnvironmentVariable("MYSQL_DATABASE");
    var mysqlUser = Environment.GetEnvironmentVariable("MYSQLUSER");
    var mysqlPassword = Environment.GetEnvironmentVariable("MYSQLPASSWORD")
        ?? Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD");

    if (string.IsNullOrWhiteSpace(mysqlHost) ||
        string.IsNullOrWhiteSpace(mysqlDatabase) ||
        string.IsNullOrWhiteSpace(mysqlUser) ||
        string.IsNullOrWhiteSpace(mysqlPassword))
    {
        throw new InvalidOperationException("MySQL connection is not configured. Set MYSQL_URL or MYSQL_PUBLIC_URL/MYSQL_ROOT_PASSWORD for Railway.");
    }

    var connectionBuilder = new StringBuilder();
    connectionBuilder.Append($"Server={mysqlHost};");
    connectionBuilder.Append($"Port={mysqlPort};");
    connectionBuilder.Append($"Database={mysqlDatabase};");
    connectionBuilder.Append($"User={mysqlUser};");
    connectionBuilder.Append($"Password={mysqlPassword};");
    connectionBuilder.Append($"SslMode={(IsLocalHost(mysqlHost) ? "None" : "Required")};");
    connectionBuilder.Append("AllowPublicKeyRetrieval=True;");

    return connectionBuilder.ToString();
}

bool IsLocalHost(string host)
{
    return host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
        || host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
        || host.Equals("::1", StringComparison.OrdinalIgnoreCase);
}
