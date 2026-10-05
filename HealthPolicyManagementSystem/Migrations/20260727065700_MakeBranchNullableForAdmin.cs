using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthPolicyManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class MakeBranchNullableForAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BranchID",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BranchID",
                table: "Users",
                column: "BranchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Branches_BranchID",
                table: "Users",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "BranchID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Branches_BranchID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BranchID",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "BranchID",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
