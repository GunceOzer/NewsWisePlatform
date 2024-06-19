using System.ComponentModel.DataAnnotations;

namespace NewsAggregationApplication.WebApi.RequestModels;

public class ResetPasswordRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}