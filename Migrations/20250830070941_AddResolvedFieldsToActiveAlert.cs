using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Audit_api.Migrations
{
    /// <inheritdoc />
    public partial class AddResolvedFieldsToActiveAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPUTime",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "Command",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "DatabaseName",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "DiskIO",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "LastRequestEndTime",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "NetAddress",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "SPIDNumber",
                table: "UserSessions");

            migrationBuilder.RenameColumn(
                name: "NetLibrary",
                table: "UserSessions",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "LastRequestStartTime",
                table: "UserSessions",
                newName: "LastRequestTime");

            migrationBuilder.RenameColumn(
                name: "UserSessionID",
                table: "UserSessions",
                newName: "SessionID");

            migrationBuilder.RenameColumn(
                name: "ActiveAlertID",
                table: "ActiveAlerts",
                newName: "AlertID");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "UserSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LoginTime",
                table: "UserSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AvgCPUTime",
                table: "QueryPerformance",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PercentUsed",
                table: "DatabaseMetrics",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Threshold",
                table: "AlertDefinitions",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ThresholdValue",
                table: "ActiveAlerts",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentValue",
                table: "ActiveAlerts",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserSessions",
                newName: "NetLibrary");

            migrationBuilder.RenameColumn(
                name: "LastRequestTime",
                table: "UserSessions",
                newName: "LastRequestStartTime");

            migrationBuilder.RenameColumn(
                name: "SessionID",
                table: "UserSessions",
                newName: "UserSessionID");

            migrationBuilder.RenameColumn(
                name: "AlertID",
                table: "ActiveAlerts",
                newName: "ActiveAlertID");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "UserSessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LoginTime",
                table: "UserSessions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "CPUTime",
                table: "UserSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Command",
                table: "UserSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DatabaseName",
                table: "UserSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DiskIO",
                table: "UserSessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRequestEndTime",
                table: "UserSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetAddress",
                table: "UserSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SPIDNumber",
                table: "UserSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "AvgCPUTime",
                table: "QueryPerformance",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PercentUsed",
                table: "DatabaseMetrics",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Threshold",
                table: "AlertDefinitions",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ThresholdValue",
                table: "ActiveAlerts",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentValue",
                table: "ActiveAlerts",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");
        }
    }
}
