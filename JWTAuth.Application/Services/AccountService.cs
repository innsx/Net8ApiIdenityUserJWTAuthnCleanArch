using JWTAuth.Application.Interfaces;
using JWTAuth.Domain.Entities;
using JWTAuth.Domain.Exceptions;
using JWTAuth.Domain.Requests;
using Microsoft.AspNetCore.Identity;

namespace JWTAuth.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IJwtTokenGeneration _jwtTokenGeneration;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepositories _userRepositories;

        public AccountService(IJwtTokenGeneration jwtTokenGeneration, UserManager<User> userManager, IUserRepositories userRepositories)
        {
            _jwtTokenGeneration = jwtTokenGeneration;
            _userManager = userManager;
            _userRepositories = userRepositories;
        }

        public async Task LoginRequestAsnyc(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user!, loginRequest.Password);

            if (user == null || !isPasswordValid)
            {
                throw new LoginFailedException(loginRequest.Email);
            }

            await GenerateNewAndRefreshTokenAsync(user);
        }

        private async Task GenerateNewAndRefreshTokenAsync(User user)
        {

            //Generate a JWT Token
            var (jwtToken, expirationDateInUtc) = _jwtTokenGeneration.GenerateJwtToken(user);

            //Generate a REFRESH JWT TOKEN
            var refreshTokenValue = _jwtTokenGeneration.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshTokenValue;
            user.RefreshTokenExpireAtUtc = refreshTokenExpirationDateInUtc;

            await _userManager.UpdateAsync(user);

            AppendJwtTokenInCookieHttpOnly(user, jwtToken, expirationDateInUtc, refreshTokenExpirationDateInUtc);
        }

        private void AppendJwtTokenInCookieHttpOnly(User user, string jwtToken, DateTime expirationDateInUtc, DateTime refreshTokenExpirationDateInUtc)
        {

            //APPEND JWT TOKEN to HTTPONLY local storage
            _jwtTokenGeneration.AppendTokenInCookieHttpOnlyLocalStorage("ACCESS_TOKEN", jwtToken, expirationDateInUtc);

            _jwtTokenGeneration.AppendTokenInCookieHttpOnlyLocalStorage("REFRESH_TOKEN", user.RefreshToken!, refreshTokenExpirationDateInUtc);
        }

        public async Task RefreshExpiredTokenAsync(string? refreshToken)
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

            await GenerateNewAndRefreshTokenAsync(user);
        }

        public async Task RegisterRequestAsync(RegisterRequest registerRequest)
        {
            var userExists = await _userManager.FindByEmailAsync(registerRequest.Email) != null;

            if (userExists)
            {
                throw new UserAlreadyExistsException(email: registerRequest.Email);
            }

            var userCreated = User.Create(registerRequest.Email, registerRequest.FirstName, registerRequest.LastName);

            userCreated.PasswordHash = _userManager.PasswordHasher.HashPassword(userCreated, registerRequest.Password);

            var identityCreatedResult = await _userManager.CreateAsync(userCreated);

            if (!identityCreatedResult.Succeeded)
            {
                throw new RegistrationFailedException(identityCreatedResult.Errors.Select(e => e.Description));
            }
        }
    }
}
