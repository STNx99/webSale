using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.DataAccess;

namespace WebsiteBanHang.Data
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DbInitializer> logger)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task Initialize()
        {
            try
            {
                // Apply pending migrations if they exist
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }

                // Seed roles
                await SeedRolesAsync();

                // Seed admin user
                await SeedAdminUserAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                _logger.LogInformation("Admin role created");
            }

            if (!await _roleManager.RoleExistsAsync("Moderator"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Moderator"));
                _logger.LogInformation("Moderator role created");
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
                _logger.LogInformation("User role created");
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = "admin@kireishop.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, "Admin@123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    _logger.LogInformation("Admin user created");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Error creating admin user: {Description}", error.Description);
                    }
                }
            }
            else if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                _logger.LogInformation("Added Admin role to existing admin user");
            }
        }
    }
}
