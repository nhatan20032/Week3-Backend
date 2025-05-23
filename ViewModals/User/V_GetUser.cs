﻿namespace EFCorePracticeAPI.ViewModals.User
{
    public class V_GetUser
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Fullname { get; set; }
        public required string Passwordhash { get; set; }
        public string? Refreshtoken { get; set; }
        public DateTime? Refreshtokenexpiry { get; set; }
        public List<int> RoleId { get; set; } = [];
        public List<string> RoleName { get; set; } = [];
    }
}
