using Microsoft.EntityFrameworkCore.Migrations;

namespace Docs.Data.Migrations
{
    public partial class RoleChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Delete",
                table: "MembersRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Delete",
                table: "MembersRoles",
                nullable: false,
                defaultValue: false);
        }
    }
}
