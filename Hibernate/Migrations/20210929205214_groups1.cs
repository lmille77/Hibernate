using Microsoft.EntityFrameworkCore.Migrations;

namespace Hibernate.Migrations
{
    public partial class groups1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Groups");
        }
    }
}
