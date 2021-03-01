using JWTWebApi.Models; 
using System.Security.Claims; 

namespace JWTWebApi.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userName);
        string ValidateToken(string token); 
        string ValidateTokenByIpMachine(string ipMachine);
        ClaimsPrincipal GetPrincipal(string token); 
        string GenerateRefreshToken();
        string GetUrlByAppCode(string appCode);
        int CheckLogin(string userName, string password);
        UserModel GetUserInfo(string userName, string password);

    }
}