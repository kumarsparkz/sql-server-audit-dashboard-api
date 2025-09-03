using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuditDashboard.Services;
using AuditDashboard.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SecurityController : ControllerBase
{
    private readonly ISecurityService _securityService;
    private readonly ILogger<SecurityController> _logger;

    public SecurityController(ISecurityService securityService, ILogger<SecurityController> logger)
    {
        _securityService = securityService;
        _logger = logger;
    }

    [HttpGet("events")]
    public async Task<ActionResult<List<SecurityEvent>>> GetSecurityEvents(
        [FromQuery] int? serverId = null, 
        [FromQuery] int hours = 24,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var events = await _securityService.GetSecurityEventsAsync(serverId, hours, page, pageSize);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting security events");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("events/summary")]
    public async Task<ActionResult<List<SecurityEventSummary>>> GetSecurityEventsSummary(
        [FromQuery] int? serverId = null, 
        [FromQuery] int hours = 24)
    {
        try
        {
            var summary = await _securityService.GetSecurityEventsSummaryAsync(serverId, hours);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting security events summary");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}