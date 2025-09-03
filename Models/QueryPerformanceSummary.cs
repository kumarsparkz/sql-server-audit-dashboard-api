namespace AuditDashboard.Models
{
    public class QueryPerformanceSummary
    {
        public string QueryText { get; set; } = string.Empty;
        public long ExecutionCount { get; set; }
        public long TotalReads { get; set; }
        public long TotalWrites { get; set; }
        public decimal AverageDuration { get; set; }
        public decimal MaxDuration { get; set; }
        public decimal TotalCPUTime { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public DateTime CollectionTime { get; set; }
    }
}