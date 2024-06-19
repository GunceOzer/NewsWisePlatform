using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class AccountService:IAccountService
{
   

    public AccountService()
    {
        
    }
    public async Task CreateRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to create role {roleName}");
                }
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