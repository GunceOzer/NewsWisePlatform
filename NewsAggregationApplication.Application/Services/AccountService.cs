using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class AccountService:IAccountService
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AccountService> _logger;

    public AccountService(RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> userManager, ILogger<AccountService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }
    
    public async Task CreateRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            
            try
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    _logger.LogInformation($"Creating role: {roleName}");
                    roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation($"Role {roleName} created successfully.");
                    }
                    else
                    {
                        _logger.LogError($"Error creating role {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Role {roleName} already exists. No action taken.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while creating role {roleName}.");
            }
        }

        User adminUser = new User
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            FullName = "admin",
            EmailConfirmed = true,
            
        };

        string userPWD = "Admin.123";
        var _user = await userManager.FindByEmailAsync("admin@admin.com");

        if (_user == null)
        {
            var createPowerUser = await userManager.CreateAsync(adminUser, userPWD);
            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    
   
}