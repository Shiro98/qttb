using System.Security.Claims; 

namespace WebAdmin.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userName);
        string ValidateToken(string token); 
        ClaimsPrincipal GetPrincipal(string token); 
        string GenerateRefreshToken();

    }
}