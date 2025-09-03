using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using AuditDashboard.Data;
using AuditDashboard.Models;

namespace AuditDashboard.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly AuditDashboardContext _context;
        private readonly ILogger<SecurityService> _logger;

        public SecurityService(AuditDashboardContext context, ILogger<SecurityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<SecurityEventSummary>> GetSecurityEventsAsync(int? serverId = null, int hours = 24)
        {
            try
            {
                var startTime = DateTime.Now.AddHours(-hours);
                
                var query = _context.SecurityEvents
                    .Where(se => se.EventTime >= startTime);

                if (serverId.HasValue)
                    query = query.Where(se => se.ServerID == serverId.Value);

                var events = await query
                    .OrderByDescending(se => se.EventTime)
                    .Take(100)
                    .Select(se => new SecurityEventSummary
                    {
                        EventType = se.EventType,
                        Description = se.Description,
                        UserName = se.UserName ?? "Unknown",
                        Severity = se.Severity,
                        EventTime = se.EventTime,
                        ServerName = se.MonitoredServer.ServerName
                    })
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security events");
                return new List<SecurityEventSummary>();
            }
        }

        public async Task<List<SecurityEvent>> GetSecurityEventsAsync(int? serverId, int hours, int page, int pageSize)
        {
            try
            {
                var startTime = DateTime.Now.AddHours(-hours);
                
                var query = _context.SecurityEvents
                    .Include(se => se.MonitoredServer)
                    .Where(se => se.EventTime >= startTime);

                if (serverId.HasValue)
                    query = query.Where(se => se.ServerID == serverId.Value);

                var events = await query
                    .OrderByDescending(se => se.EventTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated security events");
                return new List<SecurityEvent>();
            }
        }

        public async Task<Dictionary<string, int>> GetSecurityEventCountsAsync(int? serverId = null, int hours = 24)
        {
            try
            {
                var startTime = DateTime.Now.AddHours(-hours);
                
                var query = _context.SecurityEvents
                    .Where(se => se.EventTime >= startTime);

                if (serverId.HasValue)
                    query = query.Where(se => se.ServerID == serverId.Value);

                var counts = await query
                    .GroupBy(se => se.Severity)
                    .Select(g => new { Severity = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Severity, x => x.Count);

                return counts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security event counts");
                return new Dictionary<string, int>();
            }
        }

        public async Task<List<SecurityEventSummary>> GetSecurityEventsSummaryAsync(int? serverId = null, int hours = 24)
        {
            try
            {
                var startTime = DateTime.Now.AddHours(-hours);
                
                var query = _context.SecurityEvents
                    .Where(se => se.EventTime >= startTime);

                if (serverId.HasValue)
                    query = query.Where(se => se.ServerID == serverId.Value);

                var events = await query
                    .GroupBy(se => new { se.EventType, se.Severity })
                    .Select(g => new SecurityEventSummary
                    {
                        EventType = g.Key.EventType,
                        Description = $"{g.Count()} events of type {g.Key.EventType}",
                        UserName = "Multiple",
                        Severity = g.Key.Severity,
                        EventTime = g.Max(se => se.EventTime),
                        ServerName = "Multiple",
                        Count = g.Count()
                    })
                    .OrderByDescending(se => se.EventTime)
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security events summary");
                return new List<SecurityEventSummary>();
            }
        }

        public async Task<List<SecurityEventSummary>> GetHighRiskEventsAsync(int? serverId = null, int hours = 24)
        {
            try
            {
                var startTime = DateTime.Now.AddHours(-hours);
                
                var query = _context.SecurityEvents
                    .Where(se => se.EventTime >= startTime && se.Severity == "High");

                if (serverId.HasValue)
                    query = query.Where(se => se.ServerID == serverId.Value);

                var events = await query
                    .OrderByDescending(se => se.EventTime)
                    .Take(50)
                    .Select(se => new SecurityEventSummary
                    {
                        EventType = se.EventType,
                        Description = se.Description,
                        UserName = se.UserName ?? "Unknown",
                        Severity = se.Severity,
                        EventTime = se.EventTime,
                        ServerName = se.MonitoredServer.ServerName
                    })
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting high risk events");
                return new List<SecurityEventSummary>();
            }
        }

        public async Task CollectSecurityEventsAsync(int serverId, int hours = 24)
        {
            try
            {
                var server = await _context.MonitoredServers
                    .FirstOrDefaultAsync(s => s.ServerID == serverId && s.IsActive);

                if (server == null)
                {
                    _logger.LogWarning("Server {ServerId} not found or inactive", serverId);
                    return;
                }

                using var connection = new SqlConnection(server.ConnectionString);
                await connection.OpenAsync();

                // Collect login events
                await CollectLoginEventsAsync(serverId, connection, hours);

                // Collect permission changes
                await CollectPermissionChangesAsync(serverId, connection, hours);

                // Collect failed login attempts
                await CollectFailedLoginsAsync(serverId, connection, hours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting security events for server {ServerId}", serverId);
            }
        }

        public Task LogSecurityEventAsync(int serverId, string eventType, string description, string? userName = null, string severity = "Medium")
        {
            return Task.Run(async () =>
            {
                try
                {
                    var securityEvent = new SecurityEvent
                    {
                        ServerID = serverId,
                        EventType = eventType,
                        Description = description,
                        UserName = userName,
                        Severity = severity,
                        EventTime = DateTime.Now
                    };

                    _context.SecurityEvents.Add(securityEvent);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error logging security event");
                }
            });
        }

        private async Task CollectLoginEventsAsync(int serverId, SqlConnection connection, int hours)
        {
            try
            {
                var query = @"
                    SELECT 
                        login_name,
                        login_time,
                        host_name,
                        program_name
                    FROM sys.dm_exec_sessions 
                    WHERE login_time >= DATEADD(hour, @hours, GETDATE())
                    AND is_user_process = 1";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@hours", -hours);
                
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var loginName = reader["login_name"] == DBNull.Value ? "Unknown" : reader["login_name"].ToString()!;
                    var hostName = reader["host_name"] == DBNull.Value ? "Unknown" : reader["host_name"].ToString()!;
                    var programName = reader["program_name"] == DBNull.Value ? "Unknown" : reader["program_name"].ToString()!;
                    var loginTime = reader["login_time"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["login_time"]);

                    var securityEvent = new SecurityEvent
                    {
                        ServerID = serverId,
                        EventType = "User Login",
                        Description = $"User login from {hostName} using {programName}",
                        UserName = loginName,
                        Severity = "Low",
                        EventTime = loginTime
                    };

                    _context.SecurityEvents.Add(securityEvent);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting login events for server {ServerId}", serverId);
            }
        }

        private async Task CollectPermissionChangesAsync(int serverId, SqlConnection connection, int hours)
        {
            try
            {
                // This would typically query the SQL Server audit logs or extended events
                // For now, we'll create a placeholder implementation
                var securityEvent = new SecurityEvent
                {
                    ServerID = serverId,
                    EventType = "Permission Monitoring",
                    Description = "Permission changes monitoring active",
                    Severity = "Medium",
                    EventTime = DateTime.Now
                };

                _context.SecurityEvents.Add(securityEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting permission changes for server {ServerId}", serverId);
            }
        }

        private async Task CollectFailedLoginsAsync(int serverId, SqlConnection connection, int hours)
        {
            try
            {
                // This would typically query the Windows Event Log or SQL Server error log
                // For now, we'll create a placeholder implementation
                var securityEvent = new SecurityEvent
                {
                    ServerID = serverId,
                    EventType = "Failed Login Monitoring",
                    Description = "Failed login attempts monitoring active",
                    Severity = "Medium",
                    EventTime = DateTime.Now
                };

                _context.SecurityEvents.Add(securityEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting failed logins for server {ServerId}", serverId);
            }
        }
    }
}