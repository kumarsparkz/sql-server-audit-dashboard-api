namespace AuditDashboard.Models
{
    public class DashboardOverviewDto
    {
        public int TotalServers { get; set; }
        public int ActiveServers { get; set; }
        public int CriticalAlerts { get; set; }
        public int WarningAlerts { get; set; }
        public List<MetricSummary> RecentMetrics { get; set; } = new List<MetricSummary>();
        public List<DatabaseSummary> DatabaseSummaries { get; set; } = new List<DatabaseSummary>();
        public Dictionary<string, int> SecurityEventCounts { get; set; } = new Dictionary<string, int>();
    }
}