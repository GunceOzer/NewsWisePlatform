using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Models;

namespace NewsAggregationApplication.UI.Controllers;

public class AccountController : Controller
{
    
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<AccountController> _logger;
    
    
    
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<Guid>> roleManager, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]//preventing CSRF attacks which is the attacker causes the victim user to carry out an action unintentionally
    public async Task<IActionResult> Register(RegisterViewModel model)
    {if (ModelState.IsValid)
        {
            var user = new User { UserName = model.Email, Email = model.Email, FullName = model.FullName };
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User registration failed");
                ModelState.AddModelError("", "An unexpected error occurred.");
            }
        }
        return View(model);
        /*if (ModelState.IsValid)
        {
            var user = new User { UserName = model.Email, Email = model.Email, FullName = model.FullName};
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Check if the User role exists and create if not
                var userRoleExists = await _roleManager.RoleExistsAsync("User");
                if (!userRoleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
                }

                // Add user to the User role
                await _userManager.AddToRoleAsync(user, "User");

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("index", "home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If we got this far, something failed, redisplay form
        return View(model);*/
            /*if (result.Succeeded)
            {
                // after successful registration, sign the user in
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = "User" });
                }


                await _userManager.AddToRoleAsync(user, "User");
                
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("index", "home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // if something fails redisplay the form
        return View(model);*/
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        /*if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

        
        return View(model);*/
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    _logger.LogWarning("User login failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login attempt failed.");
                ModelState.AddModelError("", "An unexpected error occurred.");
            }
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        /*await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");*/
        try
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Article");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging out.");
            return RedirectToAction("Index", "Article"); // Redirect user even on error to avoid confusion
        }
    }

}