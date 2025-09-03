using System.ComponentModel.DataAnnotations;

namespace AuditDashboard.Models
{
    public class SecurityEvent
    {
        [Key]
        public long SecurityEventID { get; set; }
        public int ServerID { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Details { get; set; }
        public string Severity { get; set; } = "Medium";
        public DateTime EventTime { get; set; }

        // Navigation property
        public virtual MonitoredServer MonitoredServer { get; set; } = null!;
    }
}