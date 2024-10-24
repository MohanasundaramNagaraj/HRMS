using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class VIG_22OCT2024_06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProjectUserRole",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Business Analyst" },
                    { 2, "Technical Architect" },
                    { 3, "Project Manager" },
                    { 4, "Developer" },
                    { 5, "Tester" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProjectUserRole",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProjectUserRole",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProjectUserRole",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProjectUserRole",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProjectUserRole",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
