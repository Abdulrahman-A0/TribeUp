using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixChatRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatMessages_AspNetUsers_UserId",
                table: "GroupChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_GroupChatMessages_LastMessageId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_GroupChatMessages_GroupId",
                table: "GroupChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "GroupProfilePicture",
                table: "Groups",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "Groups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Groups",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "GroupChatMessages",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "GroupChatMessages",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LastMessageSentAt",
                table: "Groups",
                column: "LastMessageSentAt");

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessages_GroupId_SentAt",
                table: "GroupChatMessages",
                columns: new[] { "GroupId", "SentAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatMessages_AspNetUsers_UserId",
                table: "GroupChatMessages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_GroupChatMessages_LastMessageId",
                table: "Groups",
                column: "LastMessageId",
                principalTable: "GroupChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatMessages_AspNetUsers_UserId",
                table: "GroupChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_GroupChatMessages_LastMessageId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_LastMessageSentAt",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_GroupChatMessages_GroupId_SentAt",
                table: "GroupChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "GroupProfilePicture",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "GroupChatMessages",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "GroupChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessages_GroupId",
                table: "GroupChatMessages",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatMessages_AspNetUsers_UserId",
                table: "GroupChatMessages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_GroupChatMessages_LastMessageId",
                table: "Groups",
                column: "LastMessageId",
                principalTable: "GroupChatMessages",
                principalColumn: "Id");
        }
    }
}
