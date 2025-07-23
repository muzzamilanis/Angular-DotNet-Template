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

        public AuthService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto input)
        {
            // Find user by email or name
            var user = await _context.Users
                .FirstOrDefaultAsync(u => (u.Email == input.UsernameOrEmail || u.Name == input.UsernameOrEmail) && u.IsActive);

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
                Surname = "" // User model doesn't have surname, keeping empty for compatibility
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
            // In a real application, you would get the current user from the JWT token
            // For now, returning a generic response - this should be enhanced to extract from JWT
            return new SessionInfoDto
            {
                User = new UserInfoDto
                {
                    Id = 1,
                    UserName = "current-user",
                    EmailAddress = "user@example.com",
                    Name = "Current",
                    Surname = "User"
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
                    new Claim(ClaimTypes.Email, user.EmailAddress)
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