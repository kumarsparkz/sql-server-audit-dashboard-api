namespace AuditDashboard.Models
{
    public class MetricSummary
    {
        public string MetricType { get; set; } = string.Empty;
        public string? MetricName { get; set; }
        public decimal AverageValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal MinValue { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}