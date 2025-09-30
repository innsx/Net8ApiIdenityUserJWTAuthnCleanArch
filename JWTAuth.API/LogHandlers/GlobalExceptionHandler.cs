using JWTAuth.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace JWTAuth.API.LogHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, message) = GetExceptionDetails(exception);

            _logger.LogError(exception, exception.Message);

            httpContext.Response.StatusCode = (int)statusCode;

            await httpContext.Response.WriteAsJsonAsync(message, cancellationToken);

            return true;
        }

        public (HttpStatusCode statusCode, string message) GetExceptionDetails(Exception exception)
        {
            return exception switch
            {
                LoginFailedException => (HttpStatusCode.Unauthorized, exception.Message),
                UserAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),
                RegisterationFailedException => (HttpStatusCode.BadRequest, exception.Message),
                RefreshTokenException => (HttpStatusCode.Unauthorized, exception.Message),

                _ => (HttpStatusCode.InternalServerError, $"An Unexpected Error Occured: {exception.Message}")
            };
        }
    }
}
