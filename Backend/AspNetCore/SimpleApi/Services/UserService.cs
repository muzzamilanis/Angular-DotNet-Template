using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace SimpleApi.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<List<UserDto>> GetByCompanyAsync(int companyId);
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(CreateUserDto input);
        Task<UserDto> UpdateAsync(UpdateUserDto input);
        Task DeleteAsync(int id);
        Task<UserDto> ActivateAsync(int id);
        Task<UserDto> DeactivateAsync(int id);
        Task ResetPasswordAsync(ChangePasswordDto input);
        
        // Writer Portal methods
        Task<List<UserDto>> GetWritersAsync(int companyId);
        Task<List<UserDto>> GetSuperAdminsAsync(int companyId);
        Task<List<UserDto>> GetAvailableWritersAsync(int companyId);
        Task<UserDto> GetUserByIdAsync(int userId);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _context.Users
                .Include(u => u.Company)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    IsSuperAdmin = u.IsSuperAdmin,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    CompanyId = u.CompanyId,
                    CompanyName = u.Company != null ? u.Company.Name : null
                })
                .ToListAsync();

            return users;
        }

        public async Task<List<UserDto>> GetByCompanyAsync(int companyId)
        {
            var users = await _context.Users
                .Include(u => u.Company)
                .Where(u => u.CompanyId == companyId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    IsSuperAdmin = u.IsSuperAdmin,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    CompanyId = u.CompanyId,
                    CompanyName = u.Company != null ? u.Company.Name : null
                })
                .ToListAsync();

            return users;
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new ArgumentException("User not found");

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive,
                IsSuperAdmin = user.IsSuperAdmin,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                CompanyId = user.CompanyId,
                CompanyName = user.Company?.Name
            };
        }

        public async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == input.Email);
            if (existingUser != null)
                throw new ArgumentException("User with this email already exists");

            // Verify company exists
            var company = await _context.Companies.FindAsync(input.CompanyId);
            if (company == null)
                throw new ArgumentException("Company not found");

            var hashedPassword = HashPassword(input.Password);

            var user = new User
            {
                Name = input.Name,
                Email = input.Email,
                PasswordHash = hashedPassword,
                IsSuperAdmin = input.IsSuperAdmin,
                Role = input.Role,
                CompanyId = input.CompanyId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive,
                IsSuperAdmin = user.IsSuperAdmin,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                CompanyId = user.CompanyId,
                CompanyName = company.Name
            };
        }

        public async Task<UserDto> UpdateAsync(UpdateUserDto input)
        {
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == input.Id);

            if (user == null)
                throw new ArgumentException("User not found");

            // Check if email already exists for another user
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == input.Email && u.Id != input.Id);
            if (existingUser != null)
                throw new ArgumentException("User with this email already exists");

            // Verify company exists
            var company = await _context.Companies.FindAsync(input.CompanyId);
            if (company == null)
                throw new ArgumentException("Company not found");

            user.Name = input.Name;
            user.Email = input.Email;
            user.IsActive = input.IsActive;
            user.IsSuperAdmin = input.IsSuperAdmin;
            user.Role = input.Role;
            user.CompanyId = input.CompanyId;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive,
                IsSuperAdmin = user.IsSuperAdmin,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                CompanyId = user.CompanyId,
                CompanyName = company.Name
            };
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new ArgumentException("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDto> ActivateAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new ArgumentException("User not found");

            user.IsActive = true;
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive,
                IsSuperAdmin = user.IsSuperAdmin,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                CompanyId = user.CompanyId,
                CompanyName = user.Company?.Name
            };
        }

        public async Task<UserDto> DeactivateAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new ArgumentException("User not found");

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive,
                IsSuperAdmin = user.IsSuperAdmin,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                CompanyId = user.CompanyId,
                CompanyName = user.Company?.Name
            };
        }

        public async Task ResetPasswordAsync(ChangePasswordDto input)
        {
            var user = await _context.Users.FindAsync(input.UserId);
            if (user == null)
                throw new ArgumentException("User not found");

            user.PasswordHash = HashPassword(input.NewPassword);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserDto>> GetWritersAsync(int companyId)
        {
            var writers = await _context.Users
                .Include(u => u.Company)
                .Where(u => u.CompanyId == companyId && u.Role == UserRole.Writer && u.IsActive)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    IsSuperAdmin = u.IsSuperAdmin,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    CompanyId = u.CompanyId,
                    CompanyName = u.Company != null ? u.Company.Name : null
                })
                .ToListAsync();

            return writers;
        }

        public async Task<List<UserDto>> GetSuperAdminsAsync(int companyId)
        {
            var superAdmins = await _context.Users
                .Include(u => u.Company)
                .Where(u => u.CompanyId == companyId && u.IsSuperAdmin && u.IsActive)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    IsSuperAdmin = u.IsSuperAdmin,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    CompanyId = u.CompanyId,
                    CompanyName = u.Company != null ? u.Company.Name : null
                })
                .ToListAsync();

            return superAdmins;
        }

        public async Task<List<UserDto>> GetAvailableWritersAsync(int companyId)
        {
            var writers = await _context.Users
                .Include(u => u.Company)
                .Where(u => u.CompanyId == companyId && u.Role == UserRole.Writer && u.IsActive)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    IsSuperAdmin = u.IsSuperAdmin,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    CompanyId = u.CompanyId,
                    CompanyName = u.Company != null ? u.Company.Name : null
                })
                .ToListAsync();

            return writers;
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new ArgumentException("User not found");

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive,
                IsSuperAdmin = user.IsSuperAdmin,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                CompanyId = user.CompanyId,
                CompanyName = user.Company?.Name
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}