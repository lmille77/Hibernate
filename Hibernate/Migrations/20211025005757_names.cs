using Microsoft.EntityFrameworkCore.Migrations;

namespace Hibernate.Migrations
{
    public partial class names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SalesReps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesRepName",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SalesReps");

            migrationBuilder.DropColumn(
                name: "SalesRepName",
                table: "Groups");
        }
    }
}
