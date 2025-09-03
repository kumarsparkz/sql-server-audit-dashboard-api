using System.ComponentModel.DataAnnotations;

namespace AuditDashboard.Models
{
    public class DashboardUser
    {
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}