using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class VIG_22OCT2024_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ModuleStatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStatus_ModuleStatus_ModuleStatusId",
                        column: x => x.ModuleStatusId,
                        principalTable: "ModuleStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectStatus_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SprintStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    SprintId = table.Column<int>(type: "int", nullable: true),
                    ModuleStatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintStatus_ModuleStatus_ModuleStatusId",
                        column: x => x.ModuleStatusId,
                        principalTable: "ModuleStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SprintStatus_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SprintStatus_Sprint_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprint",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatus_ModuleStatusId",
                table: "ProjectStatus",
                column: "ModuleStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatus_ProjectId",
                table: "ProjectStatus",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintStatus_ModuleStatusId",
                table: "SprintStatus",
                column: "ModuleStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintStatus_ProjectId",
                table: "SprintStatus",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintStatus_SprintId",
                table: "SprintStatus",
                column: "SprintId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectStatus");

            migrationBuilder.DropTable(
                name: "SprintStatus");
        }
    }
}
