using System.ComponentModel.DataAnnotations;

namespace AuditDashboard.Models
{
    public class QueryPerformance
    {
        [Key]
        public long QueryPerformanceID { get; set; }
        public int ServerID { get; set; }
        public string? QueryText { get; set; }
        public string? QueryHash { get; set; }
        public decimal AvgDuration { get; set; }
        public long ExecutionCount { get; set; }
        public long TotalReads { get; set; }
        public long TotalWrites { get; set; }
        public decimal AvgCPUTime { get; set; }
        public DateTime? LastExecutionTime { get; set; }
        public DateTime CollectionTime { get; set; }

        // Navigation property
        public virtual MonitoredServer MonitoredServer { get; set; } = null!;
    }
}