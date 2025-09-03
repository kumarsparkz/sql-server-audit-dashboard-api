using System.ComponentModel.DataAnnotations;

namespace AuditDashboard.Models
{
    public class UserSession
    {
        [Key]
        public long SessionID { get; set; }
        public int ServerID { get; set; }
        public string? UserName { get; set; }
        public string? LoginName { get; set; }
        public string? HostName { get; set; }
        public string? ProgramName { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LastRequestTime { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CollectionTime { get; set; }

        // Navigation property
        public virtual MonitoredServer MonitoredServer { get; set; } = null!;
    }
}