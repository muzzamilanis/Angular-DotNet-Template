using System.ComponentModel.DataAnnotations;
using SimpleApi.Models;

namespace SimpleApi.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpireTime { get; set; }
        public UserInfoDto User { get; set; } = new UserInfoDto();
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;
        public bool IsSuperAdmin { get; set; } = false;
        public UserRole Role { get; set; }
        public int BusinessNatureId { get; set; }
    }

    public class SessionInfoDto
    {
        public UserInfoDto? User { get; set; }
        public ApplicationInfoDto Application { get; set; } = new ApplicationInfoDto();
    }

    public class ApplicationInfoDto
    {
        public string Version { get; set; } = "1.0.0";
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
    }
}