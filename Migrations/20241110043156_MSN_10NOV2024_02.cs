using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_10NOV2024_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "SubStatusMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SubStatusMaster",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "SubStatusMaster",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SubStatusMaster",
                type: "datetime2",
                nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "DevOPsID",
            //    table: "EmployeeTimeSheet",
            //    type: "int",
            //    nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SubStatusMaster");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SubStatusMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SubStatusMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SubStatusMaster");

            //migrationBuilder.DropColumn(
            //    name: "DevOPsID",
            //    table: "EmployeeTimeSheet");
        }
    }
}
