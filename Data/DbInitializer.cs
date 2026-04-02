using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.DataAccess;
using WebsiteBanHang.Models;

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
                // Try migrations first; if provider-specific migrations are unavailable,
                // fall back to EnsureCreated for first-time container deployment.
                if (_db.Database.GetPendingMigrations().Any())
                {
                    try
                    {
                        _db.Database.Migrate();
                    }
                    catch (Exception migrationEx)
                    {
                        _logger.LogWarning(migrationEx, "Migration failed. Falling back to EnsureCreated().");
                        await _db.Database.EnsureCreatedAsync();
                    }
                }
                else
                {
                    await _db.Database.EnsureCreatedAsync();
                }

                // Seed roles
                await SeedRolesAsync();

                // Seed sample categories and products for first run
                await SeedCatalogDataAsync();

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

        private async Task SeedCatalogDataAsync()
        {
            if (await _db.Categories.AnyAsync() || await _db.Products.AnyAsync())
            {
                _logger.LogInformation("Catalog data already exists. Skip seeding sample data.");
                return;
            }

            var categories = new List<Category>
            {
                new() { Id = 0, Name = "Skincare" },
                new() { Id = 0, Name = "Makeup" },
                new() { Id = 0, Name = "Hair Care" }
            };

            await _db.Categories.AddRangeAsync(categories);
            await _db.SaveChangesAsync();

            var categoryByName = categories.ToDictionary(c => c.Name, c => c.Id);

            var products = new List<Product>
            {
                new()
                {
                    Id = 0,
                    Name = "Sakura Gentle Cleanser",
                    Price = 12.99m,
                    Description = "Gentle daily facial cleanser for sensitive skin.",
                    CategoryId = categoryByName["Skincare"],
                    ImageUrl = "/images/sample-cleanser.jpg"
                },
                new()
                {
                    Id = 0,
                    Name = "Hydra Boost Serum",
                    Price = 24.50m,
                    Description = "Hydrating serum with lightweight texture.",
                    CategoryId = categoryByName["Skincare"],
                    ImageUrl = "/images/sample-serum.jpg"
                },
                new()
                {
                    Id = 0,
                    Name = "Velvet Matte Lipstick",
                    Price = 15.75m,
                    Description = "Long-wear matte lipstick with rich pigment.",
                    CategoryId = categoryByName["Makeup"],
                    ImageUrl = "/images/sample-lipstick.jpg"
                },
                new()
                {
                    Id = 0,
                    Name = "Silk Repair Shampoo",
                    Price = 18.20m,
                    Description = "Repair shampoo for dry and damaged hair.",
                    CategoryId = categoryByName["Hair Care"],
                    ImageUrl = "/images/sample-shampoo.jpg"
                }
            };

            await _db.Products.AddRangeAsync(products);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Sample catalog data seeded successfully.");
        }
    }
}
