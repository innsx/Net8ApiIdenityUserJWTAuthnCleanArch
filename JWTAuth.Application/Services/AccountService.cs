using JWTAuth.Application.Interfaces;
using JWTAuth.Domain.Entities;
using JWTAuth.Domain.Exceptions;
using JWTAuth.Domain.Requests;
using Microsoft.AspNetCore.Identity;

namespace JWTAuth.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAuthTokenProcessor _authTokenProcess;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepositories _userRepositories;

        public AccountService(IAuthTokenProcessor authTokenProcess, UserManager<User> userManager, IUserRepositories userRepositories)
        {
            _authTokenProcess = authTokenProcess;
            this._userManager = userManager;
            _userRepositories = userRepositories;
        }

        public async Task LoginRequestAsnyc(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (user == null || !isPasswordValid)
            {
                throw new LoginFailedException(loginRequest.Email);
            }

            await GetNewRefreshToken(user);
        }

        private async Task GetNewRefreshToken(User user)
        {

            //Create JWT Token & Refresh Token
            var (jwtToken, expirationDateInUtc) = _authTokenProcess.GenerateJwtToken(user);

            var refreshTokenValue = _authTokenProcess.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshTokenValue;
            user.RefreshTokenExpireAtUtc = refreshTokenExpirationDateInUtc;

            await _userManager.UpdateAsync(user);

            _authTokenProcess.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUtc);

            _authTokenProcess.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
        }

        public async Task RefreshTokenAsync(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
               throw new RefreshTokenException("Refresh Token is missing.");
            }

            var user = await _userRepositories.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null)
            {
                throw new RefreshTokenException("Unable to retrieve user for a refresh token.");
            }

            if (user.RefreshTokenExpireAtUtc < DateTime.UtcNow)
            {
                throw new RefreshTokenException("Refresh Token is expired.");
            }

            await GetNewRefreshToken(user);
        }

        public async Task RegisterRequestAsync(RegisterRequest registerRequest)
        {
            var userExists = await _userManager.FindByEmailAsync(registerRequest.Email) != null;

            if (userExists)
            {
                throw new UserAlreadyExistsException(email: registerRequest.Email);
            }

            var createUser = User.Create(registerRequest.Email, registerRequest.FirstName, registerRequest.LastName);

            createUser.PasswordHash = _userManager.PasswordHasher.HashPassword(createUser, registerRequest.Password);

            var result = await _userManager.CreateAsync(createUser);

            if (!result.Succeeded)
            {
                throw new RegisterationFailedException(result.Errors.Select(e => e.Description));
            }
        }
    }
}
