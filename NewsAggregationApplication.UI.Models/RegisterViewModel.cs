using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregationApplication.UI.Models;

public class RegisterViewModel
{
    [Required]
    [Display(Name="Full Name")]
    public string FullName { get; set; }
    
    [Required]
    [EmailAddress]
    [Display(Name="Email")]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [StringLength(100,ErrorMessage = "The {0} must be at least {2} and at max {1} characters long",MinimumLength = 6)]
    public string Password { get; set; }
    
    [Required]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}