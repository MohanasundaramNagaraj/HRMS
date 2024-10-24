using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class VIG_22OCT2024_05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    ProjectRoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeRole_MST_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "MST_Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_EmployeeRole_ProjectMember_MemberId",
                        column: x => x.MemberId,
                        principalTable: "ProjectMember",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeRole_ProjectUserRole_ProjectRoleId",
                        column: x => x.ProjectRoleId,
                        principalTable: "ProjectUserRole",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeRole_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRole_EmployeeId",
                table: "EmployeeRole",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRole_MemberId",
                table: "EmployeeRole",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRole_ProjectId",
                table: "EmployeeRole",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRole_ProjectRoleId",
                table: "EmployeeRole",
                column: "ProjectRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeRole");
        }
    }
}
