namespace EFCorePracticeAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string? Refreshtoken { get; set; }

    public DateTime? Refreshtokenexpiry { get; set; }

    public string? Fullname { get; set; }

    public virtual ICollection<Userrole> Userroles { get; set; } = new List<Userrole>();
}
