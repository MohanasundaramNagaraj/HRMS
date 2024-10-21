using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class VIG_16OCT2024_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_LookUp_StatusId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Sprint_LookUp_StatusId",
                table: "Sprint");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItem_LookUp_StatusId",
                table: "WorkItem");

            migrationBuilder.DropTable(
                name: "LookUp");

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplaySequence = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleStatus_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Module",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleStatus_ModuleId",
                table: "ModuleStatus",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ModuleStatus_StatusId",
                table: "Project",
                column: "StatusId",
                principalTable: "ModuleStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sprint_ModuleStatus_StatusId",
                table: "Sprint",
                column: "StatusId",
                principalTable: "ModuleStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItem_ModuleStatus_StatusId",
                table: "WorkItem",
                column: "StatusId",
                principalTable: "ModuleStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_ModuleStatus_StatusId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Sprint_ModuleStatus_StatusId",
                table: "Sprint");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItem_ModuleStatus_StatusId",
                table: "WorkItem");

            migrationBuilder.DropTable(
                name: "ModuleStatus");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.CreateTable(
                name: "LookUp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LookupType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    lookupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookUp", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Project_LookUp_StatusId",
                table: "Project",
                column: "StatusId",
                principalTable: "LookUp",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sprint_LookUp_StatusId",
                table: "Sprint",
                column: "StatusId",
                principalTable: "LookUp",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItem_LookUp_StatusId",
                table: "WorkItem",
                column: "StatusId",
                principalTable: "LookUp",
                principalColumn: "Id");
        }
    }
}
