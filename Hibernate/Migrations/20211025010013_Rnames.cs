using Microsoft.EntityFrameworkCore.Migrations;

namespace Hibernate.Migrations
{
    public partial class Rnames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesRepName",
                table: "Groups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesRepName",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
