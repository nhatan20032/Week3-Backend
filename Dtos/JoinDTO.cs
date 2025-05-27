namespace EFCorePracticeAPI.Dtos
{
    public class JoinDTO
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public List<int> RoleId { get; set; } = [];
        public List<string> RoleName { get; set; } = [];
    }
}
