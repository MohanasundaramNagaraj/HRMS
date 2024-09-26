using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_26SEP2024_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "EmployeeAttendance",
                newName: "CheckOutPosition");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "EmployeeAttendance",
                newName: "CheckInPosition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckOutPosition",
                table: "EmployeeAttendance",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "CheckInPosition",
                table: "EmployeeAttendance",
                newName: "Latitude");
        }
    }
}
