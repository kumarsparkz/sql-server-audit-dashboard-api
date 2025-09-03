namespace AuditDashboard.Services
{
    using AuditDashboard.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMonitoringService
    {
        Task CollectServerMetricsAsync(int serverId);
        Task CollectQueryPerformanceAsync(int serverId);
        Task CollectUserSessionsAsync(int serverId);
        Task<List<QueryPerformanceSummary>> GetTopSlowQueriesAsync(int? serverId = null, int hours = 24);
    }
}