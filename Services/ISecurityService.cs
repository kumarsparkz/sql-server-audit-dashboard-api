namespace AuditDashboard.Services
{
    using AuditDashboard.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISecurityService
    {
        Task<List<SecurityEvent>> GetSecurityEventsAsync(int? serverId, int hours, int page, int pageSize);
        Task<List<SecurityEventSummary>> GetSecurityEventsSummaryAsync(int? serverId, int hours);
        Task CollectSecurityEventsAsync(int serverId, int hours = 1);
    }
}