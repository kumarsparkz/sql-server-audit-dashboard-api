// This file defines the LoginRequest class, which represents the data required for user login.

namespace AuditDashboard.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}