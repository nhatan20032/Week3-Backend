using EFCorePracticeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCorePracticeAPI.Data;

// Lớp DbContext đại diện cho phiên làm việc với CSDL, ánh xạ entity -> bảng
public partial class AppDbContext : DbContext
{
    // Constructor mặc định (dành cho trường hợp không dùng DI)
    public AppDbContext()
    {
    }

    // Constructor nhận DbContextOptions (dùng khi cấu hình qua DI)
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }


    // DbSet đại diện cho bảng trong CSDL
    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    // Cấu hình chuỗi kết nối nếu không truyền từ bên ngoài
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ql_nhansu;Username=postgres;Password=123456789");

    // Cấu hình ánh xạ entity tới tên bảng cụ thể trong CSDL
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ánh xạ entity tới các bảng
        modelBuilder.Entity<Role>().ToTable("roles");

        modelBuilder.Entity<Userrole>().ToTable("userroles");

        modelBuilder.Entity<User>().ToTable("users");

        modelBuilder.Entity<RefreshToken>().ToTable("refreshtokens");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
