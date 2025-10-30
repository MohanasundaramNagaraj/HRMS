using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkHRMS.Migrations
{
    /// <inheritdoc />
    public partial class MSN_29OCT2025_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "LeaveTypeId",
            //    table: "LeaveRequest");

            //migrationBuilder.AddColumn<int>(
            //    name: "CreatedBy",
            //    table: "MST_Employee",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "CreatedDate",
            //    table: "MST_Employee",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "DateOfReleving",
            //    table: "MST_Employee",
            //    type: "datetime2",
            //    nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsActive",
            //    table: "MST_Employee",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "LastUpdatedDate",
            //    table: "MST_Employee",
            //    type: "datetime2",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "UpdatedBy",
            //    table: "MST_Employee",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "LeaveRequestId",
            //    table: "LeaveRequestDetail",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0,
            //    oldClrType: typeof(int),
            //    oldType: "int",
            //    oldNullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "Id",
            //    table: "LeaveRequestDetail",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0)
            //    .Annotation("SqlServer:Identity", "1, 1");

            //migrationBuilder.AddColumn<decimal>(
            //    name: "AllocatedDays",
            //    table: "LeaveRequestDetail",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "BalanceDays",
            //    table: "LeaveRequestDetail",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsActive",
            //    table: "LeaveRequestDetail",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<int>(
            //    name: "LeaveTypeId",
            //    table: "LeaveRequestDetail",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "RequiredDays",
            //    table: "LeaveRequestDetail",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "UsedDays",
            //    table: "LeaveRequestDetail",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "TotalLeaveDays",
            //    table: "LeaveRequest",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "UsedDays",
            //    table: "LeaveAllocationDetail",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "RemainingDays",
            //    table: "LeaveAllocationDetail",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "AllocatedDays",
            //    table: "LeaveAllocationDetail",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "TotalUsedDays",
            //    table: "LeaveAllocation",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "TotalLeaveBalance",
            //    table: "LeaveAllocation",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "TotalLeaveAllocated",
            //    table: "LeaveAllocation",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "TotalCarriedForwardLeaves",
            //    table: "LeaveAllocation",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_LeaveRequestDetail",
            //    table: "LeaveRequestDetail",
            //    column: "Id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LeaveRequestDetail_LeaveRequestId",
            //    table: "LeaveRequestDetail",
            //    column: "LeaveRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequestDetail",
                table: "LeaveRequestDetail");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequestDetail_LeaveRequestId",
                table: "LeaveRequestDetail");

            //migrationBuilder.DropColumn(
            //    name: "CreatedBy",
            //    table: "MST_Employee");

            //migrationBuilder.DropColumn(
            //    name: "CreatedDate",
            //    table: "MST_Employee");

            //migrationBuilder.DropColumn(
            //    name: "DateOfReleving",
            //    table: "MST_Employee");

            //migrationBuilder.DropColumn(
            //    name: "IsActive",
            //    table: "MST_Employee");

            //migrationBuilder.DropColumn(
            //    name: "LastUpdatedDate",
            //    table: "MST_Employee");

            //migrationBuilder.DropColumn(
            //    name: "UpdatedBy",
            //    table: "MST_Employee");

            //migrationBuilder.DropColumn(
            //    name: "Id",
            //    table: "LeaveRequestDetail");

            //migrationBuilder.DropColumn(
            //    name: "AllocatedDays",
            //    table: "LeaveRequestDetail");

            //migrationBuilder.DropColumn(
            //    name: "BalanceDays",
            //    table: "LeaveRequestDetail");

            //migrationBuilder.DropColumn(
            //    name: "IsActive",
            //    table: "LeaveRequestDetail");

            //migrationBuilder.DropColumn(
            //    name: "LeaveTypeId",
            //    table: "LeaveRequestDetail");

            //migrationBuilder.DropColumn(
            //    name: "RequiredDays",
            //    table: "LeaveRequestDetail");

            //migrationBuilder.DropColumn(
            //    name: "UsedDays",
            //    table: "LeaveRequestDetail");

            //migrationBuilder.DropColumn(
            //    name: "TotalLeaveDays",
            //    table: "LeaveRequest");

            //migrationBuilder.AlterColumn<int>(
            //    name: "LeaveRequestId",
            //    table: "LeaveRequestDetail",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AddColumn<int>(
            //    name: "LeaveTypeId",
            //    table: "LeaveRequest",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AlterColumn<int>(
            //    name: "UsedDays",
            //    table: "LeaveAllocationDetail",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.AlterColumn<int>(
            //    name: "RemainingDays",
            //    table: "LeaveAllocationDetail",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.AlterColumn<int>(
            //    name: "AllocatedDays",
            //    table: "LeaveAllocationDetail",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.AlterColumn<int>(
            //    name: "TotalUsedDays",
            //    table: "LeaveAllocation",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.AlterColumn<int>(
            //    name: "TotalLeaveBalance",
            //    table: "LeaveAllocation",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.AlterColumn<int>(
            //    name: "TotalLeaveAllocated",
            //    table: "LeaveAllocation",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.AlterColumn<int>(
            //    name: "TotalCarriedForwardLeaves",
            //    table: "LeaveAllocation",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");
        }
    }
}
