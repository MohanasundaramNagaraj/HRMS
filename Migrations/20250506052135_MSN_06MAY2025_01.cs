using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_06MAY2025_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LeaveTypeId",
                table: "LeaveRequest",
                newName: "TotalLeaveDays");

            migrationBuilder.CreateTable(
                name: "LeaveRequestDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveRequestId = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false),
                    AllocatedDays = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsedDays = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceDays = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequiredDays = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequestDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequestDetail_LeaveRequest_LeaveRequestId",
                        column: x => x.LeaveRequestId,
                        principalTable: "LeaveRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequestDetail_LeaveRequestId",
                table: "LeaveRequestDetail",
                column: "LeaveRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveRequestDetail");

            migrationBuilder.RenameColumn(
                name: "TotalLeaveDays",
                table: "LeaveRequest",
                newName: "LeaveTypeId");
        }
    }
}
