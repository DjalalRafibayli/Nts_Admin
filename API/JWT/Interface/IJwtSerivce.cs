using API.JWT.Model;
using System.Security.Claims;

namespace API.JWT.Interface
{
    public interface IJwtSerivce
    {
        Tokens Authenticate(JWTUsers jWTUsers);
        Tokens GenerateRefreshToken(string username);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshTokenFirst();
    }
}
