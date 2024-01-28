using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingApp.Migrations
{
    /// <inheritdoc />
    public partial class init_2024_01_28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isLocked",
                table: "Logins",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isLocked",
                table: "Logins");
        }
    }
}
