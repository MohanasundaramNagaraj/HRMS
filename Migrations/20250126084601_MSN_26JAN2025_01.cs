using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_26JAN2025_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Set_Month",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Set_Month", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Set_Year",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Set_Year", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timesheet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false),
                    MonthId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Task = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Descreption = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    HoursWorked = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timesheet", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Set_Month",
                columns: new[] { "Id", "IsActive", "Name", "Number" },
                values: new object[,]
                {
                    { 1, true, "January", 1 },
                    { 2, true, "February", 2 },
                    { 3, true, "March", 3 },
                    { 4, true, "April", 4 },
                    { 5, true, "May", 5 },
                    { 6, true, "June", 6 },
                    { 7, true, "July", 7 },
                    { 8, true, "August", 8 },
                    { 9, true, "September", 9 },
                    { 10, true, "October", 10 },
                    { 11, true, "November", 11 },
                    { 12, true, "December", 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Set_Month");

            migrationBuilder.DropTable(
                name: "Set_Year");

            migrationBuilder.DropTable(
                name: "Timesheet");
        }
    }
}
