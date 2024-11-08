using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_05NOV2024_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHalfDayLeave",
                table: "EmployeeAttendance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLeave",
                table: "EmployeeAttendance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnDuty",
                table: "EmployeeAttendance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPermission",
                table: "EmployeeAttendance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PermissionStartTime",
                table: "EmployeeAttendance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PermissionEndTime",
                table: "EmployeeAttendance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HalfDayLeave",
                table: "EmployeeAttendance",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
               name: "DutyStartTime",
               table: "EmployeeAttendance",
               type: "datetime2",
               nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DutyEndTime",
                table: "EmployeeAttendance",
                type: "datetime2",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DutyEndTime",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "DutyStartTime",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "HalfDayLeave",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "IsHalfDayLeave",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "IsLeave",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "IsOnDuty",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "IsPermission",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "PermissionEndTime",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "PermissionStartTime",
                table: "EmployeeAttendance");
        }
    }
}
