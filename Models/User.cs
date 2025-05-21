namespace EFCorePracticeAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string? Fullname { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Userrole> Userroles { get; set; } = [];
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
