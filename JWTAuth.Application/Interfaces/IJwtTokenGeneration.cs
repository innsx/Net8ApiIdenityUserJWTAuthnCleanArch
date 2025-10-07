using JWTAuth.Domain.Entities;

namespace JWTAuth.Application.Interfaces
{
    public interface IJwtTokenGeneration
    {
        (string jwtToken, DateTime expireAtUtc) GenerateJwtToken(User user);

        string GenerateRefreshToken();

        void AppendTokenInCookieHttpOnlyLocalStorage(string cookieName, string token, DateTime expiration);
    }
}
