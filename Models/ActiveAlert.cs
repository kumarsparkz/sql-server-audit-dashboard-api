using System.ComponentModel.DataAnnotations;

namespace AuditDashboard.Models
{
    public class ActiveAlert
    {
        [Key]
        public long AlertID { get; set; }
        public int ServerID { get; set; }
        public int AlertDefinitionID { get; set; }
        public string AlertMessage { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public decimal CurrentValue { get; set; }
        public decimal ThresholdValue { get; set; }
        public DateTime FirstOccurrence { get; set; }
        public DateTime LastOccurrence { get; set; }
        public int OccurrenceCount { get; set; } = 1;
        public string Status { get; set; } = "ACTIVE";
        public DateTime? AcknowledgedDate { get; set; }
        public string? AcknowledgedBy { get; set; }
        public string? Notes { get; set; }
        public string? ResolvedBy { get; set; }
        public DateTime? ResolvedDate { get; set; }

        // Navigation properties
        public virtual MonitoredServer MonitoredServer { get; set; } = null!;
        public virtual AlertDefinition AlertDefinition { get; set; } = null!;
    }
}