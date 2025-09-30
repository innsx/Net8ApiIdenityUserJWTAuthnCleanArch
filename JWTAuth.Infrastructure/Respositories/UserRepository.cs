using JWTAuth.Application.Interfaces;
using JWTAuth.Domain.Entities;
using JWTAuth.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.Infrastructure.Respositories
{
    
    public class UserRepository : IUserRepositories
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var user = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

            return user;
        }
    }
}
