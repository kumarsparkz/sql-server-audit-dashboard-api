namespace AuditDashboard.Services
{
    using AuditDashboard.Models;
    using System.Threading.Tasks;

    public interface IAuthService
    {
        Task<LoginResponse> AuthenticateAsync(string username, string password);
        Task<LoginResponse> RefreshTokenAsync(string username);
        Task<UserInfo?> GetUserInfoAsync(string username);
    }
}