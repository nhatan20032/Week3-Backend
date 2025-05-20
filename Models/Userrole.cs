namespace EFCorePracticeAPI.Models;

public partial class Userrole
{
    public int Id { get; set; }

    public int? Userid { get; set; }

    public int? Roleid { get; set; }

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}
