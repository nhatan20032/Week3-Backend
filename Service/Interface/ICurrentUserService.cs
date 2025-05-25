namespace EFCorePracticeAPI.Service.Interface
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Username { get; }
        string? FullName { get; }
        string? Email { get; }
        string? Role { get; }
    }
}
