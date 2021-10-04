using Microsoft.EntityFrameworkCore.Migrations;

namespace Hibernate.Migrations
{
    public partial class salesRep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "GroupLeaderId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "SalesRepId",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SalesReps",
                columns: table => new
                {
                    SalesRepId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReps", x => x.SalesRepId);
                    table.ForeignKey(
                        name: "FK_SalesReps_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesReps_UserId",
                table: "SalesReps",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_SalesReps_SalesRepId",
                table: "Groups",
                column: "SalesRepId",
                principalTable: "SalesReps",
                principalColumn: "SalesRepId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_SalesReps_SalesRepId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "SalesReps");

            migrationBuilder.AlterColumn<string>(
                name: "SalesRepId",
                table: "Groups",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "GroupLeaderId",
                table: "Groups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupLeaderId",
                table: "Groups",
                column: "GroupLeaderId");

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
    }
}
