using JWTAuth.Domain.Entities;
using JWTAuth.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuth.Application.Interfaces
{
    public interface IAccountService
    {
        Task LoginRequestAsnyc(LoginRequest loginRequest);
        Task RegisterRequestAsync(RegisterRequest registerRequest);

        Task RefreshExpiredTokenAsync(string? refreshToken);
    }
}
