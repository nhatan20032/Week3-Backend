using EFCorePracticeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCorePracticeAPI.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ql_nhansu;Username=postgres;Password=123456789");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().ToTable("roles");

        modelBuilder.Entity<Userrole>().ToTable("userroles");

        modelBuilder.Entity<User>().ToTable("users");

        modelBuilder.Entity<RefreshToken>().ToTable("refreshtokens");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
