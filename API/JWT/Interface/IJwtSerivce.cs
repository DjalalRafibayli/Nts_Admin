using API.JWT.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.JWT.Interface
{
    public interface IJwtSerivce
    {
        Task<Tokens> Authenticate(JWTUsers jWTUsers);
        Task<Tokens> GenerateRefreshToken(string username, string refreshToken);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshTokenFirst();
    }
}
