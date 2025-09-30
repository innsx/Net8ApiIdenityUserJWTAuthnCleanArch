using JWTAuth.Application.Interfaces;
using JWTAuth.Domain.Requests;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor  _httpContext;

        public AccountsController(IAccountService accountService, IHttpContextAccessor httpContext)
        {
            _accountService = accountService;
            _httpContext = httpContext;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            await _accountService.RegisterRequestAsync(registerRequest);

            return Ok();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            await _accountService.LoginRequestAsnyc(loginRequest);

            return Ok();
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = _httpContext.HttpContext?.Request.Cookies["REFRESH_TOKEN"];

            await _accountService.RefreshTokenAsync(refreshToken);

            return Ok();
        }
    }
}
