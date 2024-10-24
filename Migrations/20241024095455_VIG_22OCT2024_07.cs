using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class VIG_22OCT2024_07 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_MST_Employee_MemberId",
                table: "ProjectMember");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "ProjectMember",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMember_MemberId",
                table: "ProjectMember",
                newName: "IX_ProjectMember_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_MST_Employee_EmployeeId",
                table: "ProjectMember",
                column: "EmployeeId",
                principalTable: "MST_Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_MST_Employee_EmployeeId",
                table: "ProjectMember");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "ProjectMember",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMember_EmployeeId",
                table: "ProjectMember",
                newName: "IX_ProjectMember_MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_MST_Employee_MemberId",
                table: "ProjectMember",
                column: "MemberId",
                principalTable: "MST_Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
