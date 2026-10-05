using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthPolicyManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyContentJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentJson",
                table: "Policies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentJson",
                table: "Policies");
        }
    }
}
