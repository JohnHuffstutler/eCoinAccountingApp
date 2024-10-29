using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCoinAccountingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addjournaltransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journal_Accounts_AccountId",
                table: "Journal");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "Journal");

            migrationBuilder.DropColumn(
                name: "Debit",
                table: "Journal");

            migrationBuilder.RenameColumn(
                name: "JournalNum",
                table: "Journal",
                newName: "JournalEntryId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Journal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Journal",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "JournalTransactions",
                columns: table => new
                {
                    JournalTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalTransactions", x => x.JournalTransactionId);
                    table.ForeignKey(
                        name: "FK_JournalTransactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalTransactions_Journal_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "Journal",
                        principalColumn: "JournalEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalTransactions_AccountId",
                table: "JournalTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalTransactions_JournalEntryId",
                table: "JournalTransactions",
                column: "JournalEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journal_Accounts_AccountId",
                table: "Journal",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journal_Accounts_AccountId",
                table: "Journal");

            migrationBuilder.DropTable(
                name: "JournalTransactions");

            migrationBuilder.RenameColumn(
                name: "JournalEntryId",
                table: "Journal",
                newName: "JournalNum");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Journal",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Journal",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "Journal",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Debit",
                table: "Journal",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Journal_Accounts_AccountId",
                table: "Journal",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
