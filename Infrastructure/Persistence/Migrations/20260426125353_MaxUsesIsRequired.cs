using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MaxUsesIsRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_GroupInvitation_Usage",
                table: "GroupInvitation");

            migrationBuilder.AlterColumn<int>(
                name: "MaxUses",
                table: "GroupInvitation",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_GroupInvitation_Usage",
                table: "GroupInvitation",
                sql: "MaxUses >= 1 AND UsedCount <= MaxUses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_GroupInvitation_Usage",
                table: "GroupInvitation");

            migrationBuilder.AlterColumn<int>(
                name: "MaxUses",
                table: "GroupInvitation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AddCheckConstraint(
                name: "CK_GroupInvitation_Usage",
                table: "GroupInvitation",
                sql: "MaxUses IS NULL OR UsedCount <= MaxUses");
        }
    }
}
