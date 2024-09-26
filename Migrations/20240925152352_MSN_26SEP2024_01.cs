using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_26SEP2024_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportingHeadUserID",
                table: "MST_Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "EmployeeAttendance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "EmployeeAttendance",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportingHeadUserID",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "EmployeeAttendance");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "EmployeeAttendance");
        }
    }
}
