using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditDashboard.Data;
using AuditDashboard.Models;

namespace AuditDashboard.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ServersController : ControllerBase
    {
        private readonly AuditDashboardContext _context;
        private readonly ILogger<ServersController> _logger;

        public ServersController(AuditDashboardContext context, ILogger<ServersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<MonitoredServer>>> GetServers()
        {
            try
            {
                var servers = await _context.MonitoredServers
                    .OrderBy(s => s.ServerName)
                    .ToListAsync();

                return Ok(servers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting servers");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MonitoredServer>> CreateServer(CreateServerRequest request)
        {
            try
            {
                var server = new MonitoredServer
                {
                    ServerName = request.ServerName,
                    ConnectionString = request.ConnectionString,
                    Description = request.Description ?? string.Empty,
                    Environment = request.Environment ?? "Development",
                    IsActive = true,
                    MonitoringEnabled = true,
                    CreatedDate = DateTime.Now,
                    LastHeartbeat = DateTime.Now
                };

                _context.MonitoredServers.Add(server);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetServer), new { id = server.ServerID }, server);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating server");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MonitoredServer>> GetServer(int id)
        {
            try
            {
                var server = await _context.MonitoredServers.FindAsync(id);
                if (server == null)
                    return NotFound();

                return Ok(server);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting server {ServerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServer(int id, UpdateServerRequest request)
        {
            try
            {
                var server = await _context.MonitoredServers.FindAsync(id);
                if (server == null)
                    return NotFound();

                server.ServerName = request.ServerName ?? server.ServerName;
                server.ConnectionString = request.ConnectionString ?? server.ConnectionString;
                server.Description = request.Description ?? server.Description;
                server.Environment = request.Environment ?? server.Environment;
                server.IsActive = request.IsActive ?? server.IsActive;
                server.MonitoringEnabled = request.MonitoringEnabled ?? server.MonitoringEnabled;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating server {ServerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServer(int id)
        {
            try
            {
                var server = await _context.MonitoredServers.FindAsync(id);
                if (server == null)
                    return NotFound();

                _context.MonitoredServers.Remove(server);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting server {ServerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class CreateServerRequest
    {
        public string ServerName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Environment { get; set; }
    }

    public class UpdateServerRequest
    {
        public string? ServerName { get; set; }
        public string? ConnectionString { get; set; }
        public string? Description { get; set; }
        public string? Environment { get; set; }
        public bool? IsActive { get; set; }
        public bool? MonitoringEnabled { get; set; }
    }
}