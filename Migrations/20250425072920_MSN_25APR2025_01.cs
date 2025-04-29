using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_25APR2025_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowNegativeBalance",
                table: "SET_LeaveType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "SET_LeaveType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SET_LeaveType",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IncludeHolidaysWithLeaves",
                table: "SET_LeaveType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCarryForward",
                table: "SET_LeaveType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompensatory",
                table: "SET_LeaveType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEarnedLeave",
                table: "SET_LeaveType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEncashmentLeave",
                table: "SET_LeaveType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLeaveWithoutPay",
                table: "SET_LeaveType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptionalLeave",
                table: "SET_LeaveType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPartiallyPaidLeave",
                table: "SET_LeaveType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "SET_LeaveType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "SET_LeaveType",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "Masters",
                table: "MST_LeaveReasons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "Masters",
                table: "MST_LeaveReasons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                schema: "Masters",
                table: "MST_LeaveReasons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "Masters",
                table: "MST_LeaveReasons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "AssetMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AssetMasters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "AssetMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "AssetMasters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "AssetMaintenances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AssetMaintenances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "AssetMaintenances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "AssetMaintenances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "AssetAllocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AssetAllocations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "AssetAllocations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "AssetAllocations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowNegativeBalance",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "IncludeHolidaysWithLeaves",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "IsCarryForward",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "IsCompensatory",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "IsEarnedLeave",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "IsEncashmentLeave",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "IsLeaveWithoutPay",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "IsOptionalLeave",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "IsPartiallyPaidLeave",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SET_LeaveType");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Masters",
                table: "MST_LeaveReasons");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "Masters",
                table: "MST_LeaveReasons");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                schema: "Masters",
                table: "MST_LeaveReasons");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Masters",
                table: "MST_LeaveReasons");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetMasters");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AssetMasters");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "AssetMasters");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AssetMasters");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetMaintenances");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AssetMaintenances");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "AssetMaintenances");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AssetMaintenances");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetAllocations");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AssetAllocations");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "AssetAllocations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AssetAllocations");
        }
    }
}
