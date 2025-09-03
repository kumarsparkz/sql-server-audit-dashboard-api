namespace AuditDashboard.Models
{
    public class ServerOverviewDto
    {
        public int ServerID { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime? LastHeartbeat { get; set; }
        public int ActiveAlerts { get; set; }
        public List<MetricSummary> RecentMetrics { get; set; } = new List<MetricSummary>();
        public List<DatabaseSummary> DatabaseSummaries { get; set; } = new List<DatabaseSummary>();
    }
}