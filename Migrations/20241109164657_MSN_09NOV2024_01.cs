using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_09NOV2024_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DutyEndTime",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "DutyStartTime",
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

            migrationBuilder.CreateTable(
                name: "EmployeeAttendanceRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusUpdatedBy = table.Column<int>(type: "int", nullable: true),
                    StatusUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAttendanceRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAttendanceRequest_MST_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "MST_Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendanceRequest_EmployeeId",
                table: "EmployeeAttendanceRequest",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeAttendanceRequest");

            migrationBuilder.AddColumn<DateTime>(
                name: "DutyEndTime",
                table: "EmployeeAttendance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DutyStartTime",
                table: "EmployeeAttendance",
                type: "datetime2",
                nullable: true);

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
                name: "PermissionEndTime",
                table: "EmployeeAttendance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PermissionStartTime",
                table: "EmployeeAttendance",
                type: "datetime2",
                nullable: true);
        }
    }
}
