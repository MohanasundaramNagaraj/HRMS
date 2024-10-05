using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_02OCT2024_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportingHeadUserID",
                table: "MST_Employee");

            migrationBuilder.AddColumn<string>(
                name: "ReportingHeadMailID",
                table: "MST_Employee",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOutMadeSystemIP",
                table: "EmployeeAttendance",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportingHeadMailID",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "CheckOutMadeSystemIP",
                table: "EmployeeAttendance");

            migrationBuilder.AddColumn<int>(
                name: "ReportingHeadUserID",
                table: "MST_Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
