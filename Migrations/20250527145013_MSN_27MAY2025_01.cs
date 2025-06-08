using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_27MAY2025_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VendorName",
                table: "VisitorEntries",
                newName: "VisitorName");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "VisitorEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "VisitorEntries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "InTime",
                table: "VisitorEntries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "VisitorEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "VisitorEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OutTime",
                table: "VisitorEntries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CFG_MenuPermission",
                columns: table => new
                {
                    PermissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Permission = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CFG_MenuPermission", x => x.PermissionID);
                });

            migrationBuilder.CreateTable(
                name: "CFG_UsersAction",
                columns: table => new
                {
                    ActionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CFG_UsersAction", x => x.ActionID);
                });

            migrationBuilder.CreateTable(
                name: "SET_Menu",
                columns: table => new
                {
                    MenuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MenuName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentMenuId = table.Column<int>(type: "int", nullable: true),
                    IsSubMenu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SET_Menu", x => x.MenuID);
                });

            migrationBuilder.CreateTable(
                name: "SET_UsersActionRights",
                columns: table => new
                {
                    RightsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageCode = table.Column<string>(type: "varchar(max)", nullable: false),
                    ActionCode = table.Column<string>(type: "varchar(max)", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SET_UsersActionRights", x => x.RightsID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SET_Menu_MenuCode",
                table: "SET_Menu",
                column: "MenuCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CFG_MenuPermission");

            migrationBuilder.DropTable(
                name: "CFG_UsersAction");

            migrationBuilder.DropTable(
                name: "SET_Menu");

            migrationBuilder.DropTable(
                name: "SET_UsersActionRights");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "VisitorEntries");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "VisitorEntries");

            migrationBuilder.DropColumn(
                name: "InTime",
                table: "VisitorEntries");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "VisitorEntries");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "VisitorEntries");

            migrationBuilder.DropColumn(
                name: "OutTime",
                table: "VisitorEntries");

            migrationBuilder.RenameColumn(
                name: "VisitorName",
                table: "VisitorEntries",
                newName: "VendorName");
        }
    }
}
