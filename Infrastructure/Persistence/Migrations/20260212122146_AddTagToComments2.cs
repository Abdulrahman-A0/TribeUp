using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTagToComments2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AspNetUsers_ApplicationUserId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ApplicationUserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Tags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ApplicationUserId",
                table: "Tags",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AspNetUsers_ApplicationUserId",
                table: "Tags",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
