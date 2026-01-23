using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Post_AI_Moderation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AIModerations_AI_ModerationId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_AI_ModerationId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "AI_ModerationId",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AI_ModerationId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AI_ModerationId",
                table: "Posts",
                column: "AI_ModerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AIModerations_AI_ModerationId",
                table: "Posts",
                column: "AI_ModerationId",
                principalTable: "AIModerations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
