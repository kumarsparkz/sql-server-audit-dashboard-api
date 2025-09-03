namespace AuditDashboard.Models
{
    public class ServerStatusSummary
    {
        public int ServerID { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime? LastHeartbeat { get; set; }
        public string? ConnectionString { get; set; }
        public string? LastAlertMessage { get; set; }
        public DateTime? LastAlertTime { get; set; }
        public int CriticalAlerts { get; set; }
    }
}