using Microsoft.EntityFrameworkCore.Migrations;

namespace Docs.Data.Migrations
{
    public partial class RolesChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "View",
                table: "MembersRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "View",
                table: "MembersRoles",
                nullable: false,
                defaultValue: false);
        }
    }
}
