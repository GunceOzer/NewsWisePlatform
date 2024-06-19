using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Models;
using NewsAggregationApplication.WebApi.RequestModels;

namespace NewsAggregationApplication.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AccountsController> _logger;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;

    public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager,
        ILogger<AccountsController> logger, IJwtTokenService jwtTokenService, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
    }

    /// <summary>
    /// Authenticates a user and generates JWT and refersh tokens
    /// </summary>
    /// <param name="model">The login model contains email and password</param>
    /// <returns>JWT and refresh tokens if authentication is successful</returns>
    /// <response code="200">Returns JWT and refresh tokens.</response>
    /// <response code="400">If the login attempt is invalid.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result =
                    await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var token = _jwtTokenService.GenerateToken(user);
                    var refreshToken = _jwtTokenService.GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                    user.LastLoginDate = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    return Ok(new { Token = token, RefreshToken = refreshToken });
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return BadRequest(new { Message = "Invalid login attempt" });
    }

    /// <summary>
    /// Refreshes the JWT token using the refresh token
    /// </summary>
    /// <param name="tokenApiModel">The model contains the expired token and refresh token</param>
    /// <returns>New JWT and refresh tokens if successful</returns>
    /// <response code="200">Returns new JWT and refresh tokens.</response>
    /// <response code="400">If the refresh attempt is invalid.</response>
    [HttpPost("refresh-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody] TokenApiModel tokenApiModel)
    {
        if (ModelState.IsValid)
        {
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(tokenApiModel.Token);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.RefreshToken != tokenApiModel.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid client request");
            }

            var newToken = _jwtTokenService.GenerateToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
        }

        return BadRequest(ModelState);
    }

    /// <summary>
    /// Revokes the refresh token of the logged in user
    /// </summary>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the token is successfully revoked.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke()
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            return NotFound();
        }

        user.RefreshToken = String.Empty;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="model">The registration model contains user details Email , Fullname</param>
    /// <returns>Message indicates success of failure</returns>
    /// <response code="200">If the registration is successful.</response>
    /// <response code="400">If the registration attempt is invalid.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        if (ModelState.IsValid)
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
                    return Ok(new { Message = "Registration successful" });
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

        return BadRequest(ModelState);
    }


    /// <summary>
    /// Changes the password for the logged in user 
    /// </summary>
    /// <param name="model">The model contains the current and new password</param>
    /// <returns>Message indicates success or failure</returns>
    /// <response code="200">If the password change is successful.</response>
    /// <response code="400">If the password change attempt is invalid.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("User changed their password successfully.");
                return Ok(new { Message = "Password changed successfully" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return BadRequest(ModelState);
    }


    /// <summary>
    /// Requests a password reset for a user
    /// </summary>
    /// <param name="model">The model contains the user's email </param>
    /// <returns>Message indicates that reset instructions have been sent if the email is registered</returns>
    /// <response code="200">If the reset instructions are sent successfully.</response>
    /// <response code="400">If the reset request is invalid.</response>
    [HttpPost("reset-password-request")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // To prevent email enumeration, do not disclose whether the email is registered or not
            return Ok(new { Message = "If your email is registered, you will receive password reset instructions." });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Generate the callback URL with the correct scheme
        var callbackUrl = Url.Action("ResetPassword", "Accounts", new { token, email = user.Email },
            protocol: HttpContext.Request.Scheme);

        // Added null check for callbackUrl
        if (callbackUrl == null)
        {
            _logger.LogError("Failed to generate callback URL for password reset.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred while generating the reset URL." });
        }

        _logger.LogInformation($"Generated callback URL: {callbackUrl}");

        // Ensure the email content is properly formatted as HTML
        var emailContent = $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.";
        await _emailService.SendEmailAsync(user.Email, "Reset Password", emailContent);

        return Ok(new { Message = "If your email is registered, you will receive password reset instructions." });
    }


    
    /// <summary>
    /// Resets the password for a user using a reset token.
    /// </summary>
    /// <param name="model">The model containing the email, reset token, and new password.</param>
    /// <returns>Message indicating success or failure.</returns>
    /// <response code="200">If the password is reset successfully.</response>
    /// <response code="400">If the reset attempt is invalid.</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("Invalid request.");
        }

        var decodedToken = HttpUtility.UrlDecode(model.Token);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Password has been reset successfully." });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }


    /// <summary>
    /// Logs out the current user
    /// </summary>
    /// <returns> Message indicates success or failure</returns>
    /// <response code="200">If the logout is successful.</response>
    /// <response code="500">If an error occurs during logout.</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Ok(new { Message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging out.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred while logging out" });
        }
    }
}