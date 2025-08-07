using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Models.CRM;

namespace SimpleApi.Services.CRM
{
    public interface IContactService
    {
        Task<PagedResultDto<ContactDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<ContactDto>> GetActiveContactsAsync(int companyId);
        Task<ContactDto> GetAsync(int id, int companyId);
        Task<ContactDto> CreateAsync(CreateContactDto input, int companyId);
        Task<ContactDto> UpdateAsync(UpdateContactDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ContactService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<ContactDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.Contacts
                .Where(c => c.CompanyId == companyId)
                .Include(c => c.Lead)
                .Include(c => c.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(c => c.FirstName.Contains(input.Filter) || 
                                   c.LastName.Contains(input.Filter) || 
                                   c.Email.Contains(input.Filter));
            }

            var totalCount = await query.CountAsync();
            var contacts = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

            var dtos = _mapper.Map<List<ContactDto>>(contacts);

            return new PagedResultDto<ContactDto>(totalCount, dtos);
        }

        public async Task<List<ContactDto>> GetActiveContactsAsync(int companyId)
        {
            var contacts = await _context.Contacts
                .Include(c => c.Lead)
                .Include(c => c.Company)
                .Where(c => c.IsActive && c.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<ContactDto>>(contacts);
        }

        public async Task<ContactDto> GetAsync(int id, int companyId)
        {
            var contact = await _context.Contacts
                .Include(c => c.Lead)
                .Include(c => c.Company)
                .Where(c => c.Id == id && c.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (contact == null)
            {
                throw new ArgumentException("Contact not found");
            }
            return _mapper.Map<ContactDto>(contact);
        }

        public async Task<ContactDto> CreateAsync(CreateContactDto input, int companyId)
        {
            var contact = _mapper.Map<Contact>(input);
            contact.CompanyId = companyId;
            contact.CreatedAt = DateTime.UtcNow;
            contact.UpdatedAt = DateTime.UtcNow;
            
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            
            // Reload with includes
            await _context.Entry(contact)
                .Reference(c => c.Lead)
                .LoadAsync();
            await _context.Entry(contact)
                .Reference(c => c.Company)
                .LoadAsync();
                
            return _mapper.Map<ContactDto>(contact);
        }

        public async Task<ContactDto> UpdateAsync(UpdateContactDto input, int companyId)
        {
            var contact = await _context.Contacts
                .Include(c => c.Lead)
                .Include(c => c.Company)
                .Where(c => c.Id == input.Id && c.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (contact == null)
            {
                throw new ArgumentException("Contact not found");
            }

            _mapper.Map(input, contact);
            contact.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<ContactDto>(contact);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var contact = await _context.Contacts
                .Where(c => c.Id == id && c.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (contact == null)
            {
                throw new ArgumentException("Contact not found");
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
    }
}