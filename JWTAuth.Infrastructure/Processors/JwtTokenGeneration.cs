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
    public class JwtTokenGeneration : IJwtTokenGeneration
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTokenGeneration(IOptions<JwtOptions> jwtOptions, IHttpContextAccessor httpContextAccessor)
        {
            _jwtOptions = jwtOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        //Install MS.IdentityModel.Tokens, Systems.IdentityModel.Tokens.Jwt
        //to Generate a JWT Token & Store JWT Token 
        //in Browser's Cookie Storage as HTTPOnly/local Storage
        //install MS.AspNetCore.Http.Abstractions to ACCESS HTTP Context
        public (string jwtToken, DateTime expireAtUtc) GenerateJwtToken(User user)
        {
            var signinSecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret!));

            var signinCredentials = new SigningCredentials(signinSecretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  //JWT Token Identifier
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(ClaimTypes.NameIdentifier, user.ToString())   //user.Tostring() fetches User Model FullName toString()
                };


            //use 1 minute to TEST the expired Token; recommended time 15 min
            var expiresAt = DateTime.UtcNow.AddMinutes(1);

            //Generating the JWT Token based of these Criterias
            var token = new JwtSecurityToken
            (
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: signinCredentials
            );

            var jwtSerialized = new JwtSecurityTokenHandler().WriteToken(token);

            return (jwtSerialized, expiresAt);
        }


        //Generate a Refresh JWT Token
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();  //memory will get dispose
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        //Specified CookieOptions using HTTPContextAccessor
        // & APPEND CookieName, the token, & cookieOptions 
        public void AppendTokenInCookieHttpOnlyLocalStorage(string cookieName, string token, DateTime expiration)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                cookieName,
                token,
                new CookieOptions
                {
                    //true if a cookie must not be accessible by client-side script; otherwise, false
                    HttpOnly = true,  //specified that JWT stores in Browser' Cookie Storage as HttpOnly
                    Expires = expiration,
                    IsEssential = true, //specified that a COOKIE is Essential/must have to function properly
                    Secure = true,  //true to transmit the cookie only over an SSL connection (HTTPS)
                    SameSite = SameSiteMode.Strict //prevents the COOKIE transmits in cross sites requests
                });
        }
    }

}
