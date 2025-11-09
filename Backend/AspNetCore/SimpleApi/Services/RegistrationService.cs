using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace SimpleApi.Services
{
    public interface IRegistrationService
    {
        Task<RegistrationResponseDto> RegisterAsync(RegistrationDto registrationDto);
    }

    public class RegistrationService : IRegistrationService
    {
        private readonly ApplicationDbContext _context;

        public RegistrationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegistrationResponseDto> RegisterAsync(RegistrationDto registrationDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == registrationDto.User.Email);
                
                if (existingUser != null)
                {
                    return new RegistrationResponseDto
                    {
                        Success = false,
                        Message = "A user with this email already exists."
                    };
                }

                // Check if company already exists (optional validation)
                var existingCompany = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Name == registrationDto.Company.CompanyName);

                Company company;
                if (existingCompany != null)
                {
                    // If company exists, user can join as additional user (not super admin)
                    company = existingCompany;
                }
                else
                {
                    // Create new company
                    company = new Company
                    {
                        Name = registrationDto.Company.CompanyName,
                        Address = registrationDto.Company.Address,
                        Phone = registrationDto.Company.Phone,
                        Email = registrationDto.Company.Email,
                        ContactPerson = registrationDto.Company.ContactPerson,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        BusinessNatureId = registrationDto.Company.BusinessNatureId
                    };

                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync(); // Save to get the ID
                }

                // Check if this is the first user for the company (super admin)
                var existingUsersCount = await _context.Users
                    .CountAsync(u => u.CompanyId == company.Id);
                
                var isSuperAdmin = existingUsersCount == 0;

                // Create user
                var user = new User
                {
                    Name = registrationDto.User.Name,
                    Email = registrationDto.User.Email,
                    PasswordHash = HashPassword(registrationDto.User.Password),
                    IsActive = true,
                    IsSuperAdmin = isSuperAdmin,
                    CompanyId = company.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new RegistrationResponseDto
                {
                    Success = true,
                    Message = isSuperAdmin 
                        ? "Registration successful! You have been registered as the super admin for your company."
                        : "Registration successful! You have been added to the company.",
                    UserId = user.Id,
                    CompanyId = company.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new RegistrationResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration. Please try again."
                };
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}