namespace EFCorePracticeAPI.Service.Interface
{
    public interface ICurrentUserService
    {
        public string? UserId { get; }
        public string? Username { get; }
        public string? FullName { get; }
        public string? Email { get; }
    }
}
