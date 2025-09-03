using System.ComponentModel.DataAnnotations;

namespace AuditDashboard.Models
{
    public class ServerMetric
    {
        [Key]
        public long ServerMetricID { get; set; }
        public int ServerID { get; set; }
        public string MetricType { get; set; } = string.Empty;
        public string MetricName { get; set; } = string.Empty;
        public decimal MetricValue { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public DateTime CollectionTime { get; set; }

        // Navigation property
        public virtual MonitoredServer MonitoredServer { get; set; } = null!;
    }
}