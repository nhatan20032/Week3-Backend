using EFCorePracticeAPI.Service.Interface;
using System.Security.Claims;

namespace EFCorePracticeAPI.Service.Implement
{
    public class CurrentUserService : ICurrentUserService
    {
        public string? UserId { get; }
        public string? Username { get; }
        public string? FullName { get; }
        public string? Email { get; }
        public string? Role { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
                return;

            UserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            Username = user?.FindFirstValue(ClaimTypes.Name);
            FullName = user?.FindFirstValue(ClaimTypes.GivenName);
            Email = user?.FindFirstValue(ClaimTypes.Email);
            Role = user?.FindFirstValue(ClaimTypes.Role);
        }
    }
}
