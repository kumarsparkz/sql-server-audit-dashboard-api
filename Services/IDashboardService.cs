namespace AuditDashboard.Services
{
    using AuditDashboard.Models;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IDashboardService
    {
        Task<DashboardOverviewDto> GetDashboardOverviewAsync(int? serverId = null);
        Task<List<ServerOverviewDto>> GetServersOverviewAsync();
        Task<List<MetricSummary>> GetServerMetricsAsync(int serverId, int hours);
        Task<List<DatabaseSummary>> GetDatabaseSummaryAsync(int serverId, int days = 30);
    }
}