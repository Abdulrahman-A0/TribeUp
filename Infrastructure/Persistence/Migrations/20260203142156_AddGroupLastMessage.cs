using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupLastMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LastMessageId",
                table: "Groups",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessageSentAt",
                table: "Groups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LastMessageId",
                table: "Groups",
                column: "LastMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_GroupChatMessages_LastMessageId",
                table: "Groups",
                column: "LastMessageId",
                principalTable: "GroupChatMessages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_GroupChatMessages_LastMessageId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_LastMessageId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "LastMessageId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "LastMessageSentAt",
                table: "Groups");
        }
    }
}
