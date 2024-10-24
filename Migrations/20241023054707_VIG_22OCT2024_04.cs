using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class VIG_22OCT2024_04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_AspNetUsers_MemberId",
                table: "ProjectMember");

            migrationBuilder.CreateTable(
                name: "EmployeeReporting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportingById = table.Column<int>(type: "int", nullable: false),
                    ReportingToId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeReporting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeReporting_MST_Employee_ReportingById",
                        column: x => x.ReportingById,
                        principalTable: "MST_Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeReporting_MST_Employee_ReportingToId",
                        column: x => x.ReportingToId,
                        principalTable: "MST_Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUserRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUserRole", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeReporting_ReportingById",
                table: "EmployeeReporting",
                column: "ReportingById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeReporting_ReportingToId",
                table: "EmployeeReporting",
                column: "ReportingToId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_MST_Employee_MemberId",
                table: "ProjectMember",
                column: "MemberId",
                principalTable: "MST_Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_MST_Employee_MemberId",
                table: "ProjectMember");

            migrationBuilder.DropTable(
                name: "EmployeeReporting");

            migrationBuilder.DropTable(
                name: "ProjectUserRole");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_AspNetUsers_MemberId",
                table: "ProjectMember",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
