using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCoinAccountingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addjournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "journalNum",
                table: "Journal",
                newName: "JournalNum");

            migrationBuilder.RenameColumn(
                name: "AccountName",
                table: "Journal",
                newName: "Description");

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "Journal",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "Journal",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JournalNum",
                table: "Journal",
                newName: "journalNum");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Journal",
                newName: "AccountName");

            migrationBuilder.AlterColumn<string>(
                name: "Debit",
                table: "Journal",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Credit",
                table: "Journal",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
