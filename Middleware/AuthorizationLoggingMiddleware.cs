using Serilog;

namespace EFCorePracticeAPI.Middleware
{
    public class AuthorizationLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public AuthorizationLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                var user = context.User.Identity?.Name ?? "Anonymous";
                var path = context.Request.Path;

                Log.Warning("403 Forbidden: User '{User}' tried to access '{Path}'", user, path);
            }
            else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                var path = context.Request.Path;
                Log.Warning("401 Unauthorized access to '{Path}'", path);
            }
        }
    }
}
