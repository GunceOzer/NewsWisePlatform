using System.Text.Encodings.Web;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Models;

namespace NewsAggregationApplication.UI.Controllers;

public class AccountController : Controller
{
    
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IEmailService _emailService;
    
    
    
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _emailService = emailService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
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
                    TempData["SuccessMessage"] = "Account created successfully.";
                    return RedirectToAction("Index", "Article");
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
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index", "Article");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid login attempt. Please check your email and password.";
                    _logger.LogWarning("User login failed.");
                    return RedirectToAction("Login");
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login attempt failed.");
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return RedirectToAction("Login");
            }
        }
        //return View(model);
        return RedirectToAction("Index", "Article");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Article");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging out.");
            TempData["ErrorMessage"] = "An error occurred while logging out.";
            return RedirectToAction("Index", "Article"); 
        }
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Your password has been changed.";
            return RedirectToAction("Index", "Article");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
    
    [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { code = Uri.EscapeDataString(token) }, protocol: HttpContext.Request.Scheme);
            await _emailService.SendEmailAsync(
                model.Email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
          
            TempData["SuccessMessage"] = "Password reset email sent. Please check your email.";
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View(new ResetPasswordModel { Code = code });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            _logger.LogInformation("ResetPassword POST action called.");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError($"Validation error in {state.Key}: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogError("User not found");
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var decodedToken = Uri.UnescapeDataString(model.Code);
            _logger.LogInformation($"decoded token: {decodedToken}");
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully.";
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogError($"Error: {error.Description}");

            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
}
    
