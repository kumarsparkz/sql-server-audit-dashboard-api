namespace AuditDashboard.Models
{
    public class DatabaseSummary
    {
        public string DatabaseName { get; set; } = string.Empty;
        public decimal TotalSize { get; set; }
        public decimal UsedSpace { get; set; }
        public decimal FreeSpace { get; set; }
        public decimal PercentUsed { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}