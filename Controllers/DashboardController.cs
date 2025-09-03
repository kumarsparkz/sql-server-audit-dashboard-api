using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditDashboard.Services;
using AuditDashboard.Models;
using AuditDashboard.Data;

namespace AuditDashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly AuditDashboardContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, AuditDashboardContext context, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _context = context;
            _logger = logger;
        }

        [HttpGet("overview")]
        public async Task<ActionResult<object>> GetOverview()
        {
            try
            {
                // Get real data from database
                var totalServers = await _context.MonitoredServers.CountAsync();
                var activeServers = await _context.MonitoredServers.CountAsync(s => s.IsActive);
                var totalAlerts = await _context.ActiveAlerts.CountAsync();
                var criticalAlerts = await _context.ActiveAlerts.CountAsync(a => a.Severity == "Critical");

                // Get server statuses
                var serverStatuses = await _context.MonitoredServers
                    .Select(s => new {
                        serverName = s.ServerName,
                        status = s.IsActive ? "Online" : "Offline",
                        environment = s.Environment ?? "Unknown",
                        lastHeartbeat = s.LastHeartbeat
                    })
                    .ToListAsync();

                // Get recent alerts - using correct property names
                var recentAlerts = await _context.ActiveAlerts
                    .OrderByDescending(a => a.LastOccurrence) // Using LastOccurrence timestamp
                    .Take(5)
                    .Join(_context.MonitoredServers,
                          alert => alert.ServerID,
                          server => server.ServerID,
                          (alert, server) => new {
                              alertName = alert.AlertMessage, // Using AlertMessage instead of AlertName
                              serverName = server.ServerName,
                              severity = alert.Severity,
                              timestamp = alert.LastOccurrence, // Using LastOccurrence
                              message = alert.AlertMessage ?? "No message available" // Using AlertMessage instead of Description
                          })
                    .ToListAsync();

                var response = new
                {
                    serverStatus = new
                    {
                        totalServers = totalServers,
                        activeServers = activeServers,
                        inactiveServers = totalServers - activeServers,
                        maintenanceServers = 0
                    },
                    
                    alerts = new
                    {
                        totalAlerts = totalAlerts,
                        criticalAlerts = criticalAlerts,
                        warningAlerts = await _context.ActiveAlerts.CountAsync(a => a.Severity == "Warning"),
                        infoAlerts = await _context.ActiveAlerts.CountAsync(a => a.Severity == "Info")
                    },
                    
                    performance = new
                    {
                        avgCpuUsage = 45.2m, // Will add real metrics later
                        avgMemoryUsage = 67.8m,
                        avgDiskUsage = 58.1m,
                        connectionCount = 160
                    },
                    
                    recentActivity = recentAlerts.Take(3),
                    servers = serverStatuses,
                    databases = new object[0], // Empty for now
                    
                    chartData = new
                    {
                        cpuTrend = new object[0],
                        memoryTrend = new object[0]
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard overview");
                
                // Return fallback data if database query fails
                return Ok(new
                {
                    serverStatus = new { totalServers = 0, activeServers = 0, inactiveServers = 0, maintenanceServers = 0 },
                    alerts = new { totalAlerts = 0, criticalAlerts = 0, warningAlerts = 0, infoAlerts = 0 },
                    performance = new { avgCpuUsage = 0m, avgMemoryUsage = 0m, avgDiskUsage = 0m, connectionCount = 0 },
                    recentActivity = new object[0],
                    servers = new object[0],
                    databases = new object[0],
                    chartData = new { cpuTrend = new object[0], memoryTrend = new object[0] }
                });
            }
        }

        [HttpGet("servers")]
        public async Task<ActionResult<object[]>> GetServers()
        {
            try
            {
                var servers = await _context.MonitoredServers
                    .Select(s => new {
                        serverID = s.ServerID,
                        serverName = s.ServerName,
                        status = s.IsActive ? "Online" : "Offline",
                        environment = s.Environment ?? "Unknown",
                        lastHeartbeat = s.LastHeartbeat,
                        description = s.Description,
                        cpuUsage = 45.2m, // Default values for now
                        memoryUsage = 67.8m,
                        diskUsage = 58.1m,
                        activeConnections = 125,
                        databaseCount = 5
                    })
                    .ToListAsync();

                return Ok(servers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting servers from database");
                return Ok(new object[0]);
            }
        }

        [HttpGet("servers/{id}")]
        public async Task<ActionResult<object>> GetServerDetails(int id)
        {
            try
            {
                var server = await _context.MonitoredServers.FirstOrDefaultAsync(s => s.ServerID == id);
                
                if (server == null)
                {
                    return NotFound($"Server with ID {id} not found");
                }

                var serverDetails = new
                {
                    serverID = server.ServerID,
                    serverName = server.ServerName,
                    status = server.IsActive ? "Online" : "Offline",
                    environment = server.Environment ?? "Unknown",
                    lastHeartbeat = server.LastHeartbeat,
                    description = server.Description,
                    cpuUsage = 35.8m,
                    memoryUsage = 58.4m,
                    diskUsage = 62.1m,
                    activeConnections = 85,
                    databaseCount = 6
                };

                return Ok(serverDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting server details for ID {ServerID}", id);
                return NotFound($"Server with ID {id} not found");
            }
        }

        [HttpGet("metrics/{serverId}")]
        public async Task<ActionResult<object[]>> GetServerMetrics(int serverId, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                // Return some sample metrics for now
                var metricList = new[]
                {
                    new { metricName = "CPU Usage", averageValue = 35.8m, maxValue = 75.2m, unit = "%" },
                    new { metricName = "Memory Usage", averageValue = 58.4m, maxValue = 82.1m, unit = "%" },
                    new { metricName = "Disk Usage", averageValue = 62.1m, maxValue = 78.9m, unit = "%" }
                };

                return Ok(metricList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metrics for server {ServerID}", serverId);
                return Ok(new object[0]);
            }
        }

        [HttpGet("alerts/recent")]
        public async Task<ActionResult<object[]>> GetRecentAlerts([FromQuery] int count = 10)
        {
            try
            {
                var alerts = await _context.ActiveAlerts
                    .OrderByDescending(a => a.LastOccurrence) // Using LastOccurrence
                    .Take(count)
                    .Join(_context.MonitoredServers,
                          alert => alert.ServerID,
                          server => server.ServerID,
                          (alert, server) => new {
                              alertName = alert.AlertMessage, // Using AlertMessage
                              serverName = server.ServerName,
                              severity = alert.Severity,
                              timestamp = alert.LastOccurrence, // Using LastOccurrence
                              message = alert.AlertMessage ?? "No message available" // Using AlertMessage
                          })
                    .ToListAsync();

                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent alerts");
                return Ok(new object[0]);
            }
        }
    }
}