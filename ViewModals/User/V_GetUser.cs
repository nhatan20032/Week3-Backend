namespace EFCorePracticeAPI.ViewModals.User
{
    public class V_GetUser
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public required string Passwordhash { get; set; }       
        public List<int> RoleId { get; set; } = [];
        public List<string> RoleName { get; set; } = [];
    }
}
