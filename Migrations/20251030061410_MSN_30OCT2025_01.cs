using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_30OCT2025_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInMadeDateTime",
                table: "EmployeeAttendance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CheckInMadeUserId",
                table: "EmployeeAttendance",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutMadeDateTime",
                table: "EmployeeAttendance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CheckOutMadeUserId",
                table: "EmployeeAttendance",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInMadeDateTime",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "CheckInMadeUserId",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "CheckOutMadeDateTime",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "CheckOutMadeUserId",
                table: "EmployeeAttendance");
        }
    }
}
