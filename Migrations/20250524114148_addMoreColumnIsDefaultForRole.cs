using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCorePracticeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addMoreColumnIsDefaultForRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "roles");
        }
    }
}
