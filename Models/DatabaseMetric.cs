using System.ComponentModel.DataAnnotations;

namespace AuditDashboard.Models
{
    public class DatabaseMetric
    {
        [Key]
        public long DatabaseMetricID { get; set; }
        public int ServerID { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public decimal DatabaseSize { get; set; }
        public decimal LogSize { get; set; }
        public decimal DataSize { get; set; }
        public decimal DataUsed { get; set; } = 0;
        public decimal PercentUsed { get; set; }
        public DateTime CollectionTime { get; set; }

        // Navigation property
        public virtual MonitoredServer MonitoredServer { get; set; } = null!;
    }
}