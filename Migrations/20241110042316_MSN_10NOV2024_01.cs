using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_10NOV2024_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmployeeAttendanceRequest");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "EmployeeAttendanceRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubStatusId",
                table: "EmployeeAttendanceRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            //migrationBuilder.CreateTable(
            //    name: "EmployeeTimeSheet",
            //    columns: table => new
            //    {
            //        ID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        FromDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        ToDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        WorkItemTypeID = table.Column<int>(type: "int", nullable: false),
            //        Activity = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Descreption = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        WorkItemStatusID = table.Column<int>(type: "int", nullable: false),
            //        CreatedBy = table.Column<int>(type: "int", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedBy = table.Column<int>(type: "int", nullable: true),
            //        UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EmployeeTimeSheet", x => x.ID);
            //    });

            migrationBuilder.CreateTable(
                name: "MST_WorkItemStatus",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkItemStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descreption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MST_WorkItemStatus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MST_WorkItemType",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkItemTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MST_WorkItemType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SubStatusMaster",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StatusColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubStatusMaster", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "EmployeeTimeSheet");

            migrationBuilder.DropTable(
                name: "MST_WorkItemStatus");

            migrationBuilder.DropTable(
                name: "MST_WorkItemType");

            migrationBuilder.DropTable(
                name: "SubStatusMaster");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "EmployeeAttendanceRequest");

            migrationBuilder.DropColumn(
                name: "SubStatusId",
                table: "EmployeeAttendanceRequest");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EmployeeAttendanceRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
