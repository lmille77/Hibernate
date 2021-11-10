using Microsoft.EntityFrameworkCore.Migrations;

namespace Hibernate.Migrations
{
    public partial class fixIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Groups_GroupsGroupId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_GroupsGroupId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GroupsGroupId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "GrouptId",
                table: "Orders",
                newName: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GroupId",
                table: "Orders",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Groups_GroupId",
                table: "Orders",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Groups_GroupId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_GroupId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Orders",
                newName: "GrouptId");

            migrationBuilder.AddColumn<int>(
                name: "GroupsGroupId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GroupsGroupId",
                table: "Orders",
                column: "GroupsGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Groups_GroupsGroupId",
                table: "Orders",
                column: "GroupsGroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
