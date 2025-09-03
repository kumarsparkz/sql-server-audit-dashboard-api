namespace AuditDashboard.Models
{
    public class SecurityEventSummary
    {
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime EventTime { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public int Count { get; set; } = 1;
    }
}