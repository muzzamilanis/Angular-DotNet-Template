using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using BusinessApi.Application.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessApi.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto input);
        Task<SessionInfoDto> GetCurrentLoginInformationAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto input)
        {
            // Simple authentication - in a real application, verify against a database
            if (input.Username == "admin" && input.Password == "123qwe")
            {
                var user = new UserInfoDto
                {
                    Id = 1,
                    UserName = "admin",
                    EmailAddress = "admin@example.com",
                    Name = "System",
                    Surname = "Administrator"
                };

                var token = GenerateJwtToken(user);
                var expireTime = DateTime.UtcNow.AddHours(24);

                return new LoginResponseDto
                {
                    AccessToken = token,
                    ExpireTime = expireTime,
                    User = user
                };
            }

            throw new UnauthorizedAccessException("Invalid username or password");
        }

        public async Task<SessionInfoDto> GetCurrentLoginInformationAsync()
        {
            // In a real application, you would get the current user from the JWT token
            return new SessionInfoDto
            {
                User = new UserInfoDto
                {
                    Id = 1,
                    UserName = "admin",
                    EmailAddress = "admin@example.com",
                    Name = "System",
                    Surname = "Administrator"
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
    }
}