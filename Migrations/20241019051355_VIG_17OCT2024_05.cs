using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class VIG_17OCT2024_05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Client",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Client",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 10);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Client",
                type: "int",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 14);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Client",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 13);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 11);

            migrationBuilder.CreateIndex(
                name: "IX_Client_CreatedBy",
                table: "Client",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Client_DeletedBy",
                table: "Client",
                column: "DeletedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_AspNetUsers_CreatedBy",
                table: "Client",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Client_AspNetUsers_DeletedBy",
                table: "Client",
                column: "DeletedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_AspNetUsers_CreatedBy",
                table: "Client");

            migrationBuilder.DropForeignKey(
                name: "FK_Client_AspNetUsers_DeletedBy",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_CreatedBy",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_DeletedBy",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Client");
        }
    }
}
