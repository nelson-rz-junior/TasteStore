using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TasteStore.Models;
using TasteStore.Utility;

namespace TasteStore.DataAccess.Data.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<DbInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }

                if (!_context.Roles.Any(r => r.Name == SD.CustomerRole))
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.ManageRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.KitchenRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.FromDeskRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();
                }

                if (_context.ApplicationUsers.Count() == 0)
                {
                    await _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "admin@gmail.com",
                        Email = "admin@gmail.com",
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "Account"
                    },
                    "Admin*123");

                    ApplicationUser user = _context.ApplicationUsers.Where(u => u.Email == "admin@gmail.com")
                        .FirstOrDefault();

                    await _userManager.AddToRoleAsync(user, SD.ManageRole);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
