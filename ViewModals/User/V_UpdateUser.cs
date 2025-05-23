namespace EFCorePracticeAPI.ViewModals.User
{
    public class V_UpdateUser
    {
        public int Id { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public List<int>? RoleId { get; set; }
    }
}
