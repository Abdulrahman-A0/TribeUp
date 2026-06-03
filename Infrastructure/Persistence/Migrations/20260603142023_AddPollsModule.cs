using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPollsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PollVotes_OptionId",
                table: "PollVotes");

            migrationBuilder.DropColumn(
                name: "VotesCount",
                table: "PollOptions");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "PollVotes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PollId",
                table: "PollVotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Polls",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_ApplicationUserId",
                table: "PollVotes",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_OptionId_UserId",
                table: "PollVotes",
                columns: new[] { "OptionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_PollId",
                table: "PollVotes",
                column: "PollId");

            migrationBuilder.CreateIndex(
                name: "IX_Polls_CreatedByUserId",
                table: "Polls",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_AspNetUsers_CreatedByUserId",
                table: "Polls",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PollVotes_AspNetUsers_ApplicationUserId",
                table: "PollVotes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PollVotes_Polls_PollId",
                table: "PollVotes",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_AspNetUsers_CreatedByUserId",
                table: "Polls");

            migrationBuilder.DropForeignKey(
                name: "FK_PollVotes_AspNetUsers_ApplicationUserId",
                table: "PollVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_PollVotes_Polls_PollId",
                table: "PollVotes");

            migrationBuilder.DropIndex(
                name: "IX_PollVotes_ApplicationUserId",
                table: "PollVotes");

            migrationBuilder.DropIndex(
                name: "IX_PollVotes_OptionId_UserId",
                table: "PollVotes");

            migrationBuilder.DropIndex(
                name: "IX_PollVotes_PollId",
                table: "PollVotes");

            migrationBuilder.DropIndex(
                name: "IX_Polls_CreatedByUserId",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "PollVotes");

            migrationBuilder.DropColumn(
                name: "PollId",
                table: "PollVotes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Polls");

            migrationBuilder.AddColumn<int>(
                name: "VotesCount",
                table: "PollOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_OptionId",
                table: "PollVotes",
                column: "OptionId");
        }
    }
}
