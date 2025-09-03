using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuditDashboard.Data;
using AuditDashboard.Models;

namespace AuditDashboard.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuditDashboardContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AuditDashboardContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for username: {Username}", request.Username);

                // Find user by username
                var user = await _context.DashboardUsers
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning("User not found: {Username}", request.Username);
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        Token = string.Empty,
                        User = null
                    };
                }

                // Verify password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Invalid password for user: {Username}", request.Username);
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        Token = string.Empty,
                        User = null
                    };
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                _logger.LogInformation("Login successful for user: {Username}", request.Username);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    User = new UserInfo
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        Email = user.Email ?? string.Empty,
                        Role = user.Role
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", request.Username);
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during login",
                    Token = string.Empty,
                    User = null
                };
            }
        }

        public async Task<LoginResponse> AuthenticateAsync(string username, string password)
        {
            try
            {
                var user = await _context.DashboardUsers
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        Token = string.Empty,
                        User = null
                    };
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                
                if (!isPasswordValid)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        Token = string.Empty,
                        User = null
                    };
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Authentication successful",
                    Token = token,
                    User = new UserInfo
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        Email = user.Email ?? string.Empty,
                        Role = user.Role
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for username: {Username}", username);
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during authentication",
                    Token = string.Empty,
                    User = null
                };
            }
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? "default-secret-key-for-development-only-make-it-very-long";
                var key = Encoding.UTF8.GetBytes(secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<UserInfo?> GetUserFromTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                
                var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                    return null;

                var user = await _context.DashboardUsers
                    .FirstOrDefaultAsync(u => u.UserID == userId && u.IsActive);

                if (user == null)
                    return null;

                return new UserInfo
                {
                    UserID = user.UserID,
                    Username = user.Username,
                    Email = user.Email ?? string.Empty,
                    Role = user.Role
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserInfo?> GetUserInfoAsync(string token)
        {
            return await GetUserFromTokenAsync(token);
        }

        public async Task<LoginResponse> RefreshTokenAsync(string token)
        {
            try
            {
                var userInfo = await GetUserFromTokenAsync(token);
                if (userInfo == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid token",
                        Token = string.Empty,
                        User = null
                    };
                }

                var user = await _context.DashboardUsers
                    .FirstOrDefaultAsync(u => u.UserID == userInfo.UserID && u.IsActive);

                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "User not found or inactive",
                        Token = string.Empty,
                        User = null
                    };
                }

                // Generate new token
                var newToken = GenerateJwtToken(user);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Token = newToken,
                    User = new UserInfo
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        Email = user.Email ?? string.Empty,
                        Role = user.Role
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return new LoginResponse
                {
                    Success = false,
                    Message = "Error refreshing token",
                    Token = string.Empty,
                    User = null
                };
            }
        }

        private string GenerateJwtToken(DashboardUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "default-secret-key-for-development-only-make-it-very-long";
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.UserID.ToString()),
                    new Claim("username", user.Username),
                    new Claim("email", user.Email ?? string.Empty),
                    new Claim("role", user.Role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"] ?? "60")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}