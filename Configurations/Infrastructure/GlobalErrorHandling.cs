using Configurations.GenericApiResponse;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UserSystem.Services;

namespace Configurations.Infrastructure
{
    public class GlobalErrorHandling : BaseLogger, IExceptionHandler
    {
        private readonly ILogger<GlobalErrorHandling> _logger;
        public GlobalErrorHandling(ILogger<GlobalErrorHandling> logger) : base(logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Log exception details
            _logger.LogError(exception, "An error occurred while processing the request. Path: {Path}, Method: {Method}, User: {User}",
                httpContext.Request.Path,
                httpContext.Request.Method,
                httpContext.User?.Identity?.Name ?? "Anonymous");

            var error = new ApiResponse<object>(false, null, "An unexpected error occurred. Please try again later.");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);

            return true;
        }
    }
}
