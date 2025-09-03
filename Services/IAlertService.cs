namespace AuditDashboard.Services
{
    using AuditDashboard.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAlertService
    {
        Task<List<ActiveAlert>> GetActiveAlertsAsync(int? serverId = null);
        Task<List<AlertDefinition>> GetAlertDefinitionsAsync();
        Task ProcessAlertsAsync(int? serverId = null);
        Task AcknowledgeAlertAsync(long alertId, string acknowledgedBy, string? notes = null);
        Task ResolveAlertAsync(long alertId, string resolvedBy, string? notes = null);
    }
}