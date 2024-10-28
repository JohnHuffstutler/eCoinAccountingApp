using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCoinAccountingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentPathToJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "Journal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "Journal");
        }
    }
}
