using Microsoft.EntityFrameworkCore.Migrations;

namespace Hibernate.Migrations
{
    public partial class groupFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Groups",
                newName: "GroupId");

            migrationBuilder.AddColumn<string>(
                name: "GroupLeaderId",
                table: "Groups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesRepId",
                table: "Groups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupLeaderId",
                table: "Groups",
                column: "GroupLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_SalesRepId",
                table: "Groups",
                column: "SalesRepId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_GroupLeaderId",
                table: "Groups",
                column: "GroupLeaderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_SalesRepId",
                table: "Groups",
                column: "SalesRepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_GroupLeaderId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_SalesRepId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_GroupLeaderId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_SalesRepId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupLeaderId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SalesRepId",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Groups",
                newName: "Id");
        }
    }
}
