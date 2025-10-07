using JWTAuth.Application.Interfaces;
using JWTAuth.Domain.Entities;
using JWTAuth.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuth.Infrastructure.Processors
{
    public class AuthTokenProcessor : IAuthTokenProcessor
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenProcessor(IOptions<JwtOptions> jwtOptions, IHttpContextAccessor httpContextAccessor)
        {
            _jwtOptions = jwtOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        //Generating a JWT Token
        public (string jwtToken, DateTime expireAtUtc) GenerateJwtToken(User user)
        {
            var signinSecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret!));

            var signinCredentials = new SigningCredentials(signinSecretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(ClaimTypes.NameIdentifier, user.ToString())
                };

            var expiresAt = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken
            (
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiresAt
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return (jwtToken, expiresAt);
        }


        //Generate a Refresh JWT Token
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        //specifie & APPEND CookieName, the token, & cookieOptions to HTTPContext
        public void AppendTokenInHttpOnlyLocalStorage(string cookieName, string token, DateTime expiration)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                cookieName,
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Expires = expiration,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict //prevents cross site access
                });
        }
    }

}
