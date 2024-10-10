﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCoinAccountingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Passwordexpiration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordExpirationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordExpirationDate",
                table: "AspNetUsers");
        }
    }
}
