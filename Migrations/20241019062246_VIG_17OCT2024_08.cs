using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class VIG_17OCT2024_08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_AspNetUsers_ModifiedUserBy",
                table: "Client");

            migrationBuilder.RenameColumn(
                name: "ModifiedUserBy",
                table: "Client",
                newName: "ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Client_ModifiedUserBy",
                table: "Client",
                newName: "IX_Client_ModifiedBy");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedTime",
                table: "Client",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_AspNetUsers_ModifiedBy",
                table: "Client",
                column: "ModifiedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_AspNetUsers_ModifiedBy",
                table: "Client");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "Client",
                newName: "ModifiedUserBy");

            migrationBuilder.RenameIndex(
                name: "IX_Client_ModifiedBy",
                table: "Client",
                newName: "IX_Client_ModifiedUserBy");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedTime",
                table: "Client",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Client_AspNetUsers_ModifiedUserBy",
                table: "Client",
                column: "ModifiedUserBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
