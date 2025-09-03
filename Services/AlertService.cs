using Microsoft.EntityFrameworkCore;
using AuditDashboard.Data;
using AuditDashboard.Models;

namespace AuditDashboard.Services
{
    public class AlertService : IAlertService
    {
        private readonly AuditDashboardContext _context;
        private readonly ILogger<AlertService> _logger;

        public AlertService(AuditDashboardContext context, ILogger<AlertService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ActiveAlert>> GetActiveAlertsAsync(int? serverId = null)
        {
            try
            {
                var query = _context.ActiveAlerts
                    .Include(aa => aa.AlertDefinition)
                    .Include(aa => aa.MonitoredServer)
                    .Where(aa => aa.Status == "ACTIVE" || aa.Status == "ACKNOWLEDGED");

                if (serverId.HasValue)
                    query = query.Where(aa => aa.ServerID == serverId.Value);

                return await query
                    .OrderByDescending(aa => aa.LastOccurrence)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active alerts");
                throw;
            }
        }

        public async Task<List<AlertDefinition>> GetAlertDefinitionsAsync()
        {
            try
            {
                return await _context.AlertDefinitions
                    .Where(ad => ad.IsEnabled)
                    .OrderBy(ad => ad.AlertName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alert definitions");
                throw;
            }
        }

        public async Task ProcessAlertsAsync(int? serverId = null)
        {
            try
            {
                var definitions = await GetAlertDefinitionsAsync();
                var now = DateTime.Now;

                foreach (var definition in definitions)
                {
                    await ProcessSingleAlertDefinitionAsync(definition, serverId, now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing alerts");
                throw;
            }
        }

        public async Task AcknowledgeAlertAsync(long alertId, string acknowledgedBy, string? notes = null)
        {
            try
            {
                var alert = await _context.ActiveAlerts.FindAsync(alertId);
                if (alert == null)
                    throw new ArgumentException($"Alert with ID {alertId} not found");

                alert.Status = "ACKNOWLEDGED";
                alert.AcknowledgedBy = acknowledgedBy;
                alert.AcknowledgedDate = DateTime.Now;
                if (!string.IsNullOrEmpty(notes))
                    alert.Notes = notes;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Alert {AlertId} acknowledged by {User}", alertId, acknowledgedBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acknowledging alert {AlertId}", alertId);
                throw;
            }
        }

        public async Task ResolveAlertAsync(long alertId, string resolvedBy, string? notes = null)
        {
            try
            {
                var alert = await _context.ActiveAlerts.FindAsync(alertId);
                if (alert == null)
                    throw new ArgumentException($"Alert with ID {alertId} not found");

                alert.Status = "RESOLVED";
                alert.ResolvedBy = resolvedBy;
                alert.ResolvedDate = DateTime.Now;
                if (!string.IsNullOrEmpty(notes))
                    alert.Notes = (alert.Notes ?? "") + Environment.NewLine + $"Resolved: {notes}";

                await _context.SaveChangesAsync();
                _logger.LogInformation("Alert {AlertId} resolved by {User}", alertId, resolvedBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving alert {AlertId}", alertId);
                throw;
            }
        }

        private async Task ProcessSingleAlertDefinitionAsync(AlertDefinition definition, int? serverId, DateTime now)
        {
            try
            {
                if (definition.AlertType == "PERFORMANCE" && definition.MetricName == "CPU_Percent")
                {
                    await ProcessCPUAlertAsync(definition, serverId, now);
                }
                else if (definition.AlertType == "SECURITY" && definition.MetricName == "Failed_Logins")
                {
                    await ProcessFailedLoginAlertAsync(definition, serverId, now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing alert definition {AlertName}", definition.AlertName);
            }
        }

        private async Task ProcessCPUAlertAsync(AlertDefinition definition, int? serverId, DateTime now)
        {
            var timeWindow = definition.TimeWindow ?? 5;
            var threshold = definition.Threshold ?? 80;
            var startTime = now.AddMinutes(-timeWindow);

            var avgCpuUsage = await _context.ServerMetrics
                .Where(sm => sm.MetricType == "CPU" && sm.MetricName == "ProcessorTime")
                .Where(sm => sm.CollectionTime >= startTime)
                .Where(sm => serverId == null || sm.ServerID == serverId)
                .GroupBy(sm => sm.ServerID)
                .Select(g => new { ServerID = g.Key, AvgCPU = g.Average(sm => sm.MetricValue) })
                .Where(x => x.AvgCPU > threshold)
                .ToListAsync();

            foreach (var server in avgCpuUsage)
            {
                await CreateOrUpdateAlertAsync(definition.AlertDefinitionID, server.ServerID, 
                    $"High CPU usage: {server.AvgCPU:F1}% (threshold: {threshold}%)",
                    server.AvgCPU, threshold, definition.Severity, now);
            }
        }

        private async Task ProcessFailedLoginAlertAsync(AlertDefinition definition, int? serverId, DateTime now)
        {
            await Task.CompletedTask; // Remove warning
        }

        private async Task CreateOrUpdateAlertAsync(int alertDefinitionId, int serverId, string message, 
            decimal currentValue, decimal thresholdValue, string severity, DateTime now)
        {
            var existingAlert = await _context.ActiveAlerts
                .FirstOrDefaultAsync(aa => aa.AlertDefinitionID == alertDefinitionId && 
                                          aa.ServerID == serverId && 
                                          aa.Status == "ACTIVE");

            if (existingAlert != null)
            {
                existingAlert.LastOccurrence = now;
                existingAlert.OccurrenceCount++;
                existingAlert.CurrentValue = currentValue;
                existingAlert.AlertMessage = message;
            }
            else
            {
                var newAlert = new ActiveAlert
                {
                    AlertDefinitionID = alertDefinitionId,
                    ServerID = serverId,
                    AlertMessage = message,
                    CurrentValue = currentValue,
                    ThresholdValue = thresholdValue,
                    Severity = severity,
                    FirstOccurrence = now,
                    LastOccurrence = now,
                    OccurrenceCount = 1
                };

                _context.ActiveAlerts.Add(newAlert);
            }

            await _context.SaveChangesAsync();
        }
    }
}