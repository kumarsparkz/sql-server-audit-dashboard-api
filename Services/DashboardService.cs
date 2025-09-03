using Microsoft.EntityFrameworkCore;
using AuditDashboard.Data;
using AuditDashboard.Models;

namespace AuditDashboard.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AuditDashboardContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(AuditDashboardContext context, ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardOverviewDto> GetDashboardOverviewAsync()
        {
            try
            {
                var totalServers = await _context.MonitoredServers.CountAsync(s => s.IsActive);
                var activeServers = await _context.MonitoredServers.CountAsync(s => s.IsActive && s.LastHeartbeat > DateTime.Now.AddMinutes(-5));
                
                var alerts = await _context.ActiveAlerts
                    .Where(a => a.Status == "ACTIVE")
                    .GroupBy(a => a.Severity)
                    .Select(g => new { Severity = g.Key, Count = g.Count() })
                    .ToListAsync();

                var recentMetrics = await _context.ServerMetrics
                    .Where(sm => sm.CollectionTime >= DateTime.Now.AddHours(-1))
                    .GroupBy(sm => sm.MetricType)
                    .Select(g => new MetricSummary 
                    { 
                        MetricType = g.Key, 
                        AverageValue = g.Average(m => m.MetricValue),
                        MaxValue = g.Max(m => m.MetricValue),
                        MinValue = g.Min(m => m.MetricValue)
                    })
                    .ToListAsync();

                var dbMetrics = await _context.DatabaseMetrics
                    .Where(dm => dm.CollectionTime >= DateTime.Now.AddHours(-24))
                    .GroupBy(dm => dm.DatabaseName)
                    .Select(g => new DatabaseSummary 
                    { 
                        DatabaseName = g.Key,
                        TotalSize = g.Sum(d => d.DatabaseSize),
                        UsedSpace = g.Average(d => d.DataUsed)
                    })
                    .Take(10)
                    .ToListAsync();

                var securityEvents = await _context.SecurityEvents
                    .Where(se => se.EventTime >= DateTime.Now.AddHours(-24))
                    .GroupBy(se => se.Severity)
                    .Select(g => new { Severity = g.Key, Count = g.Count() })
                    .ToListAsync();

                return new DashboardOverviewDto
                {
                    TotalServers = totalServers,
                    ActiveServers = activeServers,
                    CriticalAlerts = alerts.FirstOrDefault(a => a.Severity == "Critical")?.Count ?? 0,
                    WarningAlerts = alerts.FirstOrDefault(a => a.Severity == "Warning")?.Count ?? 0,
                    RecentMetrics = recentMetrics,
                    DatabaseSummaries = dbMetrics,
                    SecurityEventCounts = securityEvents.ToDictionary(se => se.Severity, se => se.Count)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard overview");
                throw;
            }
        }

        public async Task<DashboardOverviewDto> GetDashboardOverviewAsync(int? serverId)
        {
            try
            {
                var totalServers = serverId.HasValue 
                    ? await _context.MonitoredServers.CountAsync(s => s.IsActive && s.ServerID == serverId.Value)
                    : await _context.MonitoredServers.CountAsync(s => s.IsActive);

                var activeServers = serverId.HasValue
                    ? await _context.MonitoredServers.CountAsync(s => s.IsActive && s.ServerID == serverId.Value && s.LastHeartbeat > DateTime.Now.AddMinutes(-5))
                    : await _context.MonitoredServers.CountAsync(s => s.IsActive && s.LastHeartbeat > DateTime.Now.AddMinutes(-5));
                
                var alertsQuery = _context.ActiveAlerts.Where(a => a.Status == "ACTIVE");
                if (serverId.HasValue)
                    alertsQuery = alertsQuery.Where(a => a.ServerID == serverId.Value);

                var alerts = await alertsQuery
                    .GroupBy(a => a.Severity)
                    .Select(g => new { Severity = g.Key, Count = g.Count() })
                    .ToListAsync();

                var metricsQuery = _context.ServerMetrics.Where(sm => sm.CollectionTime >= DateTime.Now.AddHours(-1));
                if (serverId.HasValue)
                    metricsQuery = metricsQuery.Where(sm => sm.ServerID == serverId.Value);

                var recentMetrics = await metricsQuery
                    .GroupBy(sm => sm.MetricType)
                    .Select(g => new MetricSummary 
                    { 
                        MetricType = g.Key, 
                        AverageValue = g.Average(m => m.MetricValue),
                        MaxValue = g.Max(m => m.MetricValue),
                        MinValue = g.Min(m => m.MetricValue)
                    })
                    .ToListAsync();

                var dbMetricsQuery = _context.DatabaseMetrics.Where(dm => dm.CollectionTime >= DateTime.Now.AddHours(-24));
                if (serverId.HasValue)
                    dbMetricsQuery = dbMetricsQuery.Where(dm => dm.ServerID == serverId.Value);

                var dbMetrics = await dbMetricsQuery
                    .GroupBy(dm => dm.DatabaseName)
                    .Select(g => new DatabaseSummary 
                    { 
                        DatabaseName = g.Key,
                        TotalSize = g.Sum(d => d.DatabaseSize),
                        UsedSpace = g.Average(d => d.DataUsed)
                    })
                    .Take(10)
                    .ToListAsync();

                var securityEventsQuery = _context.SecurityEvents.Where(se => se.EventTime >= DateTime.Now.AddHours(-24));
                if (serverId.HasValue)
                    securityEventsQuery = securityEventsQuery.Where(se => se.ServerID == serverId.Value);

                var securityEvents = await securityEventsQuery
                    .GroupBy(se => se.Severity)
                    .Select(g => new { Severity = g.Key, Count = g.Count() })
                    .ToListAsync();

                return new DashboardOverviewDto
                {
                    TotalServers = totalServers,
                    ActiveServers = activeServers,
                    CriticalAlerts = alerts.FirstOrDefault(a => a.Severity == "Critical")?.Count ?? 0,
                    WarningAlerts = alerts.FirstOrDefault(a => a.Severity == "Warning")?.Count ?? 0,
                    RecentMetrics = recentMetrics,
                    DatabaseSummaries = dbMetrics,
                    SecurityEventCounts = securityEvents.ToDictionary(se => se.Severity, se => se.Count)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard overview for server {ServerId}", serverId);
                throw;
            }
        }

        public async Task<List<ServerOverviewDto>> GetServerOverviewAsync()
        {
            try
            {
                var servers = await _context.MonitoredServers
                    .Where(s => s.IsActive)
                    .Select(s => new ServerOverviewDto
                    {
                        ServerID = s.ServerID,
                        ServerName = s.ServerName,
                        Environment = s.Environment,
                        IsOnline = s.LastHeartbeat > DateTime.Now.AddMinutes(-5),
                        LastHeartbeat = s.LastHeartbeat
                    })
                    .ToListAsync();

                foreach (var server in servers)
                {
                    var recentMetrics = await _context.ServerMetrics
                        .Where(sm => sm.ServerID == server.ServerID && sm.CollectionTime >= DateTime.Now.AddHours(-1))
                        .GroupBy(sm => sm.MetricType)
                        .Select(g => new MetricSummary 
                        { 
                            MetricType = g.Key, 
                            AverageValue = g.Average(m => m.MetricValue),
                            MaxValue = g.Max(m => m.MetricValue),
                            MinValue = g.Min(m => m.MetricValue)
                        })
                        .ToListAsync();

                    server.RecentMetrics = recentMetrics;

                    var activeAlerts = await _context.ActiveAlerts
                        .Where(aa => aa.ServerID == server.ServerID && aa.Status == "ACTIVE")
                        .CountAsync();

                    server.ActiveAlerts = activeAlerts;

                    var dbMetrics = await _context.DatabaseMetrics
                        .Where(dm => dm.ServerID == server.ServerID && dm.CollectionTime >= DateTime.Now.AddHours(-24))
                        .GroupBy(dm => dm.DatabaseName)
                        .Select(g => new DatabaseSummary 
                        { 
                            DatabaseName = g.Key,
                            TotalSize = g.Sum(d => d.DatabaseSize),
                            UsedSpace = g.Average(d => d.DataUsed)
                        })
                        .Take(5)
                        .ToListAsync();

                    server.DatabaseSummaries = dbMetrics;
                }

                return servers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting server overview");
                throw;
            }
        }

        public async Task<List<ServerOverviewDto>> GetServersOverviewAsync()
        {
            return await GetServerOverviewAsync();
        }

        public async Task<List<MetricSummary>> GetMetricSummaryAsync(int? serverId = null, int hours = 24)
        {
            try
            {
                var startTime = DateTime.Now.AddHours(-hours);
                
                var query = _context.ServerMetrics
                    .Where(sm => sm.CollectionTime >= startTime);

                if (serverId.HasValue)
                    query = query.Where(sm => sm.ServerID == serverId.Value);

                var metrics = await query
                    .GroupBy(sm => new { sm.MetricType, sm.MetricName })
                    .Select(g => new MetricSummary
                    {
                        MetricType = g.Key.MetricType,
                        MetricName = g.Key.MetricName,
                        AverageValue = g.Average(sm => sm.MetricValue),
                        MaxValue = g.Max(sm => sm.MetricValue),
                        MinValue = g.Min(sm => sm.MetricValue),
                        LastUpdated = g.Max(sm => sm.CollectionTime)
                    })
                    .OrderBy(ms => ms.MetricType)
                    .ThenBy(ms => ms.MetricName)
                    .ToListAsync();

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metric summary");
                throw;
            }
        }

        public async Task<List<MetricSummary>> GetServerMetricsAsync(int serverId, int hours = 24)
        {
            try
            {
                var startTime = DateTime.Now.AddHours(-hours);
                
                var metrics = await _context.ServerMetrics
                    .Where(sm => sm.ServerID == serverId && sm.CollectionTime >= startTime)
                    .GroupBy(sm => new { sm.MetricType, sm.MetricName })
                    .Select(g => new MetricSummary
                    {
                        MetricType = g.Key.MetricType,
                        MetricName = g.Key.MetricName,
                        AverageValue = g.Average(sm => sm.MetricValue),
                        MaxValue = g.Max(sm => sm.MetricValue),
                        MinValue = g.Min(sm => sm.MetricValue),
                        LastUpdated = g.Max(sm => sm.CollectionTime)
                    })
                    .OrderBy(ms => ms.MetricType)
                    .ThenBy(ms => ms.MetricName)
                    .ToListAsync();

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting server metrics for server {ServerId}", serverId);
                throw;
            }
        }

        public async Task<List<DatabaseSummary>> GetDatabaseSummaryAsync(int serverId, int hours = 24)
        {
            try
            {
                var startTime = DateTime.Now.AddHours(-hours);
                
                var dbMetrics = await _context.DatabaseMetrics
                    .Where(dm => dm.ServerID == serverId && dm.CollectionTime >= startTime)
                    .GroupBy(dm => dm.DatabaseName)
                    .Select(g => new DatabaseSummary 
                    { 
                        DatabaseName = g.Key,
                        TotalSize = g.Sum(d => d.DatabaseSize),
                        UsedSpace = g.Average(d => d.DataUsed)
                    })
                    .ToListAsync();

                return dbMetrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database summary for server {ServerId}", serverId);
                throw;
            }
        }

        public async Task<List<ServerStatusSummary>> GetServerStatusSummaryAsync()
        {
            try
            {
                var servers = await _context.MonitoredServers
                    .Where(s => s.IsActive)
                    .Select(s => new ServerStatusSummary
                    {
                        ServerID = s.ServerID,
                        ServerName = s.ServerName,
                        Environment = s.Environment,
                        IsOnline = s.LastHeartbeat > DateTime.Now.AddMinutes(-5),
                        LastHeartbeat = s.LastHeartbeat,
                        ConnectionString = s.ConnectionString
                    })
                    .ToListAsync();

                foreach (var server in servers)
                {
                    var recentAlert = await _context.ActiveAlerts
                        .Where(aa => aa.ServerID == server.ServerID && aa.Status == "ACTIVE")
                        .OrderByDescending(aa => aa.LastOccurrence)
                        .FirstOrDefaultAsync();

                    if (recentAlert != null)
                    {
                        server.LastAlertMessage = recentAlert.AlertMessage;
                        server.LastAlertTime = recentAlert.LastOccurrence;
                    }

                    var criticalAlerts = await _context.ActiveAlerts
                        .Where(aa => aa.ServerID == server.ServerID && aa.Status == "ACTIVE" && aa.Severity == "Critical")
                        .CountAsync();

                    server.CriticalAlerts = criticalAlerts;
                }

                return servers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting server status summary");
                throw;
            }
        }
    }
}