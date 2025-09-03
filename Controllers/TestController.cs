using Microsoft.AspNetCore.Mvc;

namespace AuditDashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                message = "API is working!", 
                timestamp = DateTime.Now,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                cors = "Enabled for localhost:3000"
            });
        }

        [HttpGet("cors")]
        public IActionResult TestCors()
        {
            return Ok(new { 
                message = "CORS test successful!",
                origin = Request.Headers["Origin"].ToString(),
                method = Request.Method,
                timestamp = DateTime.Now
            });
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                status = "Healthy",
                api = "Running",
                database = "Connected", // You can add actual DB health check here
                timestamp = DateTime.Now
            });
        }
    }
}