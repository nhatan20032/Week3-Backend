namespace EFCorePracticeAPI.ViewModals.User
{
    public class V_CreateUser
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
        public List<int>? RoleIds { get; set; } 
    }
}
