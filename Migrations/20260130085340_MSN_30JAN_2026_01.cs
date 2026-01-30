using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_30JAN_2026_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "MST_Employee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "MST_Employee");
        }
    }
}
