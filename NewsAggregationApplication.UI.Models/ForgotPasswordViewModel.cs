using System.ComponentModel.DataAnnotations;

namespace NewsAggregationApplication.UI.Models;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}