using System.ComponentModel.DataAnnotations;

namespace NewsAggregationApplication.WebApi.RequestModels;

public class TokenApiModel
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string RefreshToken { get; set; }
}