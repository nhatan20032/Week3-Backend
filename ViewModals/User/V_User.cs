namespace EFCorePracticeAPI.ViewModals.User
{
    public class V_User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Fullname { get; set; }
        public required string Passwordhash { get; set; }
        public int RoleId { get; set; }
    }
}
