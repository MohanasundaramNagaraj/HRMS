using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_04APR_2025_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetAllocations_AssetMasters_AssetId",
                table: "AssetAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetMaintenances_AssetMasters_AssetId",
                table: "AssetMaintenances");

            migrationBuilder.DropIndex(
                name: "IX_AssetMaintenances_AssetId",
                table: "AssetMaintenances");

            migrationBuilder.DropIndex(
                name: "IX_AssetAllocations_AssetId",
                table: "AssetAllocations");

            migrationBuilder.EnsureSchema(
                name: "Masters");

            migrationBuilder.CreateTable(
                name: "MST_LeaveReasons",
                schema: "Masters",
                columns: table => new
                {
                    LeaveReasonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveReasonName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MST_LeaveReasons", x => x.LeaveReasonId);
                });

            migrationBuilder.CreateTable(
                name: "SET_LeaveType",
                columns: table => new
                {
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MaxAllowedDaysPerYear = table.Column<int>(type: "int", nullable: false),
                    ApplicableAfterWorkingDays = table.Column<int>(type: "int", nullable: false),
                    MaxConsecutiveLeaveAllowedDaysPerMonth = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SET_LeaveType", x => x.LeaveTypeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MST_LeaveReasons",
                schema: "Masters");

            migrationBuilder.DropTable(
                name: "SET_LeaveType");

            migrationBuilder.CreateIndex(
                name: "IX_AssetMaintenances_AssetId",
                table: "AssetMaintenances",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAllocations_AssetId",
                table: "AssetAllocations",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAllocations_AssetMasters_AssetId",
                table: "AssetAllocations",
                column: "AssetId",
                principalTable: "AssetMasters",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetMaintenances_AssetMasters_AssetId",
                table: "AssetMaintenances",
                column: "AssetId",
                principalTable: "AssetMasters",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
