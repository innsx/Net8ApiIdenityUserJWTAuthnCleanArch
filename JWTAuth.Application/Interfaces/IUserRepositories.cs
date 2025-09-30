using JWTAuth.Domain.Entities;

namespace JWTAuth.Application.Interfaces
{
    public interface IUserRepositories
    {
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    }
}
