using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTagToComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "Tags",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ApplicationUserId",
                table: "Tags",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CommentId",
                table: "Tags",
                column: "CommentId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Tags_PostOrComment",
                table: "Tags",
                sql: "(PostId IS NOT NULL AND CommentId IS NULL) OR (PostId IS NULL AND CommentId IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AspNetUsers_ApplicationUserId",
                table: "Tags",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Comments_CommentId",
                table: "Tags",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AspNetUsers_ApplicationUserId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Comments_CommentId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ApplicationUserId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CommentId",
                table: "Tags");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Tags_PostOrComment",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Tags");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
