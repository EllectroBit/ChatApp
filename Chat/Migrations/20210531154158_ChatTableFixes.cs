using Microsoft.EntityFrameworkCore.Migrations;

namespace Chat.Migrations
{
    public partial class ChatTableFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_FirstUserID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_SecondUserID",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_FirstUserID",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_SecondUserID",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "FirstUserID",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "SecondUserID",
                table: "Chats");

            migrationBuilder.AddColumn<string>(
                name: "FirstUser",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondUser",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstUser",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "SecondUser",
                table: "Chats");

            migrationBuilder.AddColumn<int>(
                name: "FirstUserID",
                table: "Chats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondUserID",
                table: "Chats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_FirstUserID",
                table: "Chats",
                column: "FirstUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_SecondUserID",
                table: "Chats",
                column: "SecondUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_FirstUserID",
                table: "Chats",
                column: "FirstUserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_SecondUserID",
                table: "Chats",
                column: "SecondUserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
