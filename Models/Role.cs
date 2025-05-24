namespace EFCorePracticeAPI.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsDefault { get; set; } = false;

    public virtual ICollection<Userrole> Userroles { get; set; } = new List<Userrole>();
}
