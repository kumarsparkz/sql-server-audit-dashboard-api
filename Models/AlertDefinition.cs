namespace AuditDashboard.Models
{
    public class AlertDefinition
    {
        public int AlertDefinitionID { get; set; }
        public string AlertName { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public string? MetricName { get; set; }
        public decimal? Threshold { get; set; }
        public string? Operator { get; set; }
        public int? TimeWindow { get; set; }
        public string? Description { get; set; }
        public string Severity { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public bool EmailNotification { get; set; }
        public string? EmailRecipients { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}