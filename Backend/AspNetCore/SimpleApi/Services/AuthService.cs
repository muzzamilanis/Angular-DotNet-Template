using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleApi.Data;
using SimpleApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SimpleApi.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto input);
        Task<SessionInfoDto> GetCurrentLoginInformationAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IConfiguration configuration, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto input)
        {
            // Find user by email or name (case-insensitive) and include Company
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => (u.Email.ToLower() == input.UsernameOrEmail.ToLower() || u.Name.ToLower() == input.UsernameOrEmail.ToLower()) && u.IsActive);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username/email or password");
            }

            // Verify password
            if (!VerifyPassword(input.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username/email or password");
            }

            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                UserName = user.Email, // Use email as username
                EmailAddress = user.Email,
                Name = user.Name,
				Surname = "", // User model doesn't have surname, keeping empty for compatibility
				CompanyId = user.CompanyId.ToString(), // Assuming CompanyId is a string, adjust as necessary
                IsSuperAdmin = user.IsSuperAdmin,
                Role = user.Role,
                BusinessNatureId = user.Company?.BusinessNatureId ?? 0
			};

            var token = GenerateJwtToken(userInfo);
            var expireTime = DateTime.UtcNow.AddHours(24);

            return new LoginResponseDto
            {
                AccessToken = token,
                ExpireTime = expireTime,
                User = userInfo
            };
        }

        public async Task<SessionInfoDto> GetCurrentLoginInformationAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            // Extract claims from JWT token
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }

            // Get user details from database to ensure data is current and include Company
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            return new SessionInfoDto
            {
                User = new UserInfoDto
                {
                    Id = user.Id,
                    UserName = user.Email, // Use email as username for consistency
                    EmailAddress = user.Email,
                    Name = user.Name,
                    Surname = "", // User model doesn't have surname, keeping empty for compatibility
                    CompanyId = user.CompanyId.ToString(), // Assuming CompanyId is a string, adjust as necessary
                    IsSuperAdmin = user.IsSuperAdmin,
                    Role = user.Role,
                    BusinessNatureId = user.Company?.BusinessNatureId ?? 0
				},
                Application = new ApplicationInfoDto
                {
                    Version = "1.0.0",
                    ReleaseDate = DateTime.Now
                }
            };
        }

        private string GenerateJwtToken(UserInfoDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-must-be-at-least-256-bits-long-for-security");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.EmailAddress),
                    new Claim("CompanyId", user.CompanyId),
                    new Claim("IsSuperAdmin", user.IsSuperAdmin.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "SimpleApi",
                Audience = _configuration["Jwt:Audience"] ?? "SimpleApi"
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPassword(string password, string hash)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedPassword = Convert.ToBase64String(hashedBytes);
            return hashedPassword == hash;
        }
    }
}