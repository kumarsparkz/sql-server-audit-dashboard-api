using System.ComponentModel.DataAnnotations;

namespace AuditDashboard.Models
{
    public class MonitoredServer
    {
        [Key]
        public int ServerID { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string Environment { get; set; } = "Production";
        public bool IsActive { get; set; } = true;
        public bool MonitoringEnabled { get; set; } = true;
        public DateTime? LastHeartbeat { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<ServerMetric> ServerMetrics { get; set; } = new List<ServerMetric>();
        public virtual ICollection<ActiveAlert> ActiveAlerts { get; set; } = new List<ActiveAlert>();
    }
}