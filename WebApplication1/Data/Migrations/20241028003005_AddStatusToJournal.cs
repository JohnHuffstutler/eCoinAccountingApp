using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCoinAccountingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Journal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Journal");
        }
    }
}
