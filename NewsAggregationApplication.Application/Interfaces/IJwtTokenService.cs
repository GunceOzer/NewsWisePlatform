using System.Security.Claims;
using NewsAggregationApplication.Data.Entities;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    
}