using JWTAuth.Domain.Entities;

namespace JWTAuth.Application.Interfaces
{
    public interface IAuthTokenProcessor
    {
        (string jwtToken, DateTime expireAtUtc) GenerateJwtToken(User user);

        string GenerateRefreshToken();

        void AppendTokenInHttpOnlyLocalStorage(string cookieName, string token, DateTime expiration);
    }
}
