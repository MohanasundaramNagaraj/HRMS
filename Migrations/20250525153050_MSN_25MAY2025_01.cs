using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_25MAY2025_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VisitorEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PassNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GateInCharge = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    VisitorCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PurposeOfVisit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PersonToMeet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IDProofType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IDProofNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    VisitorImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AdditionalVisitorCount = table.Column<int>(type: "int", nullable: false),
                    AdditionalVisitorNames = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RecentVisits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorEntries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitorEntries");
        }
    }
}
