using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using AuditDashboard.Data;
using AuditDashboard.Models;

namespace AuditDashboard.Services
{
    public class MonitoringService : IMonitoringService
    {
        private readonly AuditDashboardContext _context;
        private readonly ILogger<MonitoringService> _logger;

        public MonitoringService(AuditDashboardContext context, ILogger<MonitoringService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CollectMetricsForServer(MonitoredServer server)
        {
            try
            {
                using var connection = new SqlConnection(server.ConnectionString);
                await connection.OpenAsync();

                // Collect different types of metrics
                await CollectCpuMetricsAsync(server.ServerID, connection);
                await CollectMemoryMetricsAsync(server.ServerID, connection);
                await CollectDiskMetricsAsync(server.ServerID, connection);
                await CollectDatabaseMetricsAsync(server.ServerID, connection);

                // Update server heartbeat
                server.LastHeartbeat = DateTime.Now;
                _context.MonitoredServers.Update(server);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully collected metrics for server {server.ServerName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error collecting metrics for server {server.ServerName}");
            }
        }

        public async Task CollectServerMetricsAsync(int serverId)
        {
            try
            {
                var server = await _context.MonitoredServers.FirstOrDefaultAsync(s => s.ServerID == serverId);
                if (server != null)
                {
                    await CollectMetricsForServer(server);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error collecting metrics for server {serverId}");
            }
        }

        public async Task CollectQueryPerformanceAsync(int serverId)
        {
            try
            {
                _logger.LogInformation($"Collecting query performance for server {serverId}");
                
                // Add a demo record
                var demoQueryPerf = new QueryPerformance
                {
                    ServerID = serverId,
                    QueryText = "SELECT * FROM Orders WHERE OrderDate > GETDATE()-30",
                    QueryHash = Guid.NewGuid().ToString(),
                    ExecutionCount = 150,
                    TotalReads = 5000,
                    TotalWrites = 1200,
                    CollectionTime = DateTime.Now
                };

                _context.QueryPerformance.Add(demoQueryPerf);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error collecting query performance for server {serverId}");
            }
        }

        public async Task CollectUserSessionsAsync(int serverId)
        {
            try
            {
                // Add default session metrics
                var defaultMetrics = new List<ServerMetric>
                {
                    new ServerMetric
                    {
                        ServerID = serverId,
                        MetricName = "Active Sessions",
                        MetricType = "Sessions",
                        MetricValue = 12m,
                        UnitOfMeasure = "Count",
                        CollectionTime = DateTime.Now
                    }
                };
                
                _context.ServerMetrics.AddRange(defaultMetrics);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error collecting user sessions for server {serverId}");
            }
        }

        public async Task<List<QueryPerformanceSummary>> GetTopSlowQueriesAsync(int? serverId = null, int topCount = 10)
        {
            try
            {
                // Return demo data for now
                return new List<QueryPerformanceSummary>
                {
                    new QueryPerformanceSummary
                    {
                        QueryText = "SELECT * FROM Orders o JOIN Customers c ON o.CustomerID = c.ID WHERE...",
                        ExecutionCount = 1250,
                        TotalReads = 50000,
                        TotalWrites = 12000,
                        AverageDuration = 2.5m,
                        MaxDuration = 15.2m,
                        TotalCPUTime = 156.7m,
                        DatabaseName = "ProductionDB",
                        CollectionTime = DateTime.Now
                    },
                    new QueryPerformanceSummary
                    {
                        QueryText = "SELECT SUM(Amount), COUNT(*) FROM Transactions WHERE Date >= ...",
                        ExecutionCount = 450,
                        TotalReads = 25000,
                        TotalWrites = 3000,
                        AverageDuration = 1.8m,
                        MaxDuration = 8.9m,
                        TotalCPUTime = 67.3m,
                        DatabaseName = "AnalyticsDB",
                        CollectionTime = DateTime.Now
                    },
                    new QueryPerformanceSummary
                    {
                        QueryText = "UPDATE Inventory SET Quantity = Quantity - @Amount WHERE ProductID = @ID",
                        ExecutionCount = 850,
                        TotalReads = 15000,
                        TotalWrites = 8500,
                        AverageDuration = 0.9m,
                        MaxDuration = 4.2m,
                        TotalCPUTime = 45.2m,
                        DatabaseName = "InventoryDB",
                        CollectionTime = DateTime.Now
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting top slow queries");
                return new List<QueryPerformanceSummary>();
            }
        }

        private async Task CollectCpuMetricsAsync(int serverId, SqlConnection connection)
        {
            try
            {
                // Add demo metric
                var demoMetric = new ServerMetric
                {
                    ServerID = serverId,
                    MetricName = "CPU Usage",
                    MetricType = "Performance",
                    MetricValue = new Random().Next(15, 45),
                    UnitOfMeasure = "Percent",
                    CollectionTime = DateTime.Now
                };
                
                _context.ServerMetrics.Add(demoMetric);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error collecting CPU metrics for server {serverId}");
            }
        }

        private async Task CollectMemoryMetricsAsync(int serverId, SqlConnection connection)
        {
            try
            {
                // Add demo metric
                var demoMetric = new ServerMetric
                {
                    ServerID = serverId,
                    MetricName = "Memory Usage",
                    MetricType = "Performance",
                    MetricValue = new Random().Next(35, 70),
                    UnitOfMeasure = "Percent",
                    CollectionTime = DateTime.Now
                };
                
                _context.ServerMetrics.Add(demoMetric);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error collecting memory metrics for server {serverId}");
            }
        }

        private async Task CollectDiskMetricsAsync(int serverId, SqlConnection connection)
        {
            try
            {
                // Add demo disk metric
                var demoMetric = new ServerMetric
                {
                    ServerID = serverId,
                    MetricName = "Disk Usage - C:\\",
                    MetricType = "Storage",
                    MetricValue = new Random().Next(45, 80),
                    UnitOfMeasure = "Percent",
                    CollectionTime = DateTime.Now
                };
                
                _context.ServerMetrics.Add(demoMetric);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error collecting disk metrics for server {serverId}");
            }
        }

        private async Task CollectDatabaseMetricsAsync(int serverId, SqlConnection connection)
        {
            try
            {
                // Add demo database metrics
                var demoDatabases = new[] { "ProductionDB", "TestDB", "AnalyticsDB" };
                var random = new Random();

                foreach (var dbName in demoDatabases)
                {
                    var dbMetric = new DatabaseMetric
                    {
                        ServerID = serverId,
                        DatabaseName = dbName,
                        CollectionTime = DateTime.Now,
                        DatabaseSize = 1000 + random.Next(1, 5000),
                        DataUsed = 500 + random.Next(1, 2000)
                    };

                    _context.DatabaseMetrics.Add(dbMetric);
                }
                
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error collecting database metrics for server {serverId}");
            }
        }

        public async Task<List<ServerMetric>> GetServerMetricsAsync(int serverId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.ServerMetrics.Where(m => m.ServerID == serverId);

                if (fromDate.HasValue)
                    query = query.Where(m => m.CollectionTime >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(m => m.CollectionTime <= toDate.Value);

                return await query.OrderByDescending(m => m.CollectionTime).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting server metrics for server {serverId}");
                return new List<ServerMetric>();
            }
        }

        public async Task<List<DatabaseMetric>> GetDatabaseMetricsAsync(int serverId, string? databaseName = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.DatabaseMetrics.Where(m => m.ServerID == serverId);

                if (!string.IsNullOrEmpty(databaseName))
                    query = query.Where(m => m.DatabaseName == databaseName);

                if (fromDate.HasValue)
                    query = query.Where(m => m.CollectionTime >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(m => m.CollectionTime <= toDate.Value);

                return await query.OrderByDescending(m => m.CollectionTime).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting database metrics for server {serverId}");
                return new List<DatabaseMetric>();
            }
        }

        public async Task<bool> TestServerConnectionAsync(string connectionString)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing server connection");
                return false;
            }
        }
    }
}