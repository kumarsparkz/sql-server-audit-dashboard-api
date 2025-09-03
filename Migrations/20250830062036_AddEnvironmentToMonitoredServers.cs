using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Audit_api.Migrations
{
    /// <inheritdoc />
    public partial class AddEnvironmentToMonitoredServers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertDefinitions",
                columns: table => new
                {
                    AlertDefinitionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetricName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Threshold = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeWindow = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EmailNotification = table.Column<bool>(type: "bit", nullable: false),
                    EmailRecipients = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertDefinitions", x => x.AlertDefinitionID);
                });

            migrationBuilder.CreateTable(
                name: "DashboardUsers",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardUsers", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "MonitoredServers",
                columns: table => new
                {
                    ServerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Environment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MonitoringEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastHeartbeat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoredServers", x => x.ServerID);
                });

            migrationBuilder.CreateTable(
                name: "ActiveAlerts",
                columns: table => new
                {
                    ActiveAlertID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertDefinitionID = table.Column<int>(type: "int", nullable: false),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    AlertMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ThresholdValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstOccurrence = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastOccurrence = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OccurrenceCount = table.Column<int>(type: "int", nullable: false),
                    AcknowledgedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcknowledgedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveAlerts", x => x.ActiveAlertID);
                    table.ForeignKey(
                        name: "FK_ActiveAlerts_AlertDefinitions_AlertDefinitionID",
                        column: x => x.AlertDefinitionID,
                        principalTable: "AlertDefinitions",
                        principalColumn: "AlertDefinitionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveAlerts_MonitoredServers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "MonitoredServers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatabaseMetrics",
                columns: table => new
                {
                    DatabaseMetricID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatabaseSize = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LogSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataUsed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PercentUsed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollectionTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseMetrics", x => x.DatabaseMetricID);
                    table.ForeignKey(
                        name: "FK_DatabaseMetrics_MonitoredServers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "MonitoredServers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueryPerformance",
                columns: table => new
                {
                    QueryPerformanceID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    QueryText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueryHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvgDuration = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ExecutionCount = table.Column<long>(type: "bigint", nullable: false),
                    TotalReads = table.Column<long>(type: "bigint", nullable: false),
                    TotalWrites = table.Column<long>(type: "bigint", nullable: false),
                    AvgCPUTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CollectionTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryPerformance", x => x.QueryPerformanceID);
                    table.ForeignKey(
                        name: "FK_QueryPerformance_MonitoredServers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "MonitoredServers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecurityEvents",
                columns: table => new
                {
                    SecurityEventID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityEvents", x => x.SecurityEventID);
                    table.ForeignKey(
                        name: "FK_SecurityEvents_MonitoredServers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "MonitoredServers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerMetrics",
                columns: table => new
                {
                    ServerMetricID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    MetricType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetricName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetricValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerMetrics", x => x.ServerMetricID);
                    table.ForeignKey(
                        name: "FK_ServerMetrics_MonitoredServers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "MonitoredServers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    UserSessionID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerID = table.Column<int>(type: "int", nullable: false),
                    SPIDNumber = table.Column<int>(type: "int", nullable: false),
                    LoginName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProgramName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRequestStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRequestEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Command = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CPUTime = table.Column<int>(type: "int", nullable: false),
                    DiskIO = table.Column<long>(type: "bigint", nullable: false),
                    NetAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NetLibrary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CollectionTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.UserSessionID);
                    table.ForeignKey(
                        name: "FK_UserSessions_MonitoredServers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "MonitoredServers",
                        principalColumn: "ServerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveAlerts_AlertDefinitionID",
                table: "ActiveAlerts",
                column: "AlertDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveAlerts_ServerID",
                table: "ActiveAlerts",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_DatabaseMetrics_ServerID",
                table: "DatabaseMetrics",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_QueryPerformance_ServerID",
                table: "QueryPerformance",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEvents_ServerID",
                table: "SecurityEvents",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_ServerMetrics_ServerID",
                table: "ServerMetrics",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_ServerID",
                table: "UserSessions",
                column: "ServerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveAlerts");

            migrationBuilder.DropTable(
                name: "DashboardUsers");

            migrationBuilder.DropTable(
                name: "DatabaseMetrics");

            migrationBuilder.DropTable(
                name: "QueryPerformance");

            migrationBuilder.DropTable(
                name: "SecurityEvents");

            migrationBuilder.DropTable(
                name: "ServerMetrics");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "AlertDefinitions");

            migrationBuilder.DropTable(
                name: "MonitoredServers");
        }
    }
}
