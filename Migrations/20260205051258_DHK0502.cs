using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class DHK0502 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AadhaarNumber",
                table: "MST_Employee",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlternateMobileNumber",
                table: "MST_Employee",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "MST_Employee",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrivingLicenseNumber",
                table: "MST_Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyAlternateNumber",
                table: "MST_Employee",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "MST_Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactNumber",
                table: "MST_Employee",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactRelation",
                table: "MST_Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "MST_Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherName",
                table: "MST_Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "MST_Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDependents",
                table: "MST_Employee",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PANNumber",
                table: "MST_Employee",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "MST_Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpouseName",
                table: "MST_Employee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AadhaarNumber",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "AlternateMobileNumber",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseNumber",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "EmergencyAlternateNumber",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "EmergencyContactNumber",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "EmergencyContactRelation",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "MotherName",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "NumberOfDependents",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "PANNumber",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "MST_Employee");

            migrationBuilder.DropColumn(
                name: "SpouseName",
                table: "MST_Employee");
        }
    }
}
