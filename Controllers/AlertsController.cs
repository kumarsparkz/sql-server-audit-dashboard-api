using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuditDashboard.Services;
using AuditDashboard.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(IAlertService alertService, ILogger<AlertsController> logger)
    {
        _alertService = alertService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActiveAlert>>> GetActiveAlerts([FromQuery] int? serverId = null)
    {
        try
        {
            var alerts = await _alertService.GetActiveAlertsAsync(serverId);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active alerts");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("{alertId}/acknowledge")]
    public async Task<ActionResult> AcknowledgeAlert(long alertId, [FromBody] AcknowledgeAlertRequest request)
    {
        try
        {
            var username = User.Identity?.Name ?? "Unknown";
            await _alertService.AcknowledgeAlertAsync(alertId, username, request.Notes);
            return Ok(new { message = "Alert acknowledged successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acknowledging alert {AlertId}", alertId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("definitions")]
    public async Task<ActionResult<List<AlertDefinition>>> GetAlertDefinitions()
    {
        try
        {
            var definitions = await _alertService.GetAlertDefinitionsAsync();
            return Ok(definitions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alert definitions");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

public class AcknowledgeAlertRequest
{
    public string Notes { get; set; } = string.Empty;
}