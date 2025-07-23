using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<ProjectListDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.Projects
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.SaleOrders)
                .AsQueryable();

            // Apply filtering
            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(p => p.ProjectName.Contains(input.Filter) || 
                                        (p.ProjectDescription != null && p.ProjectDescription.Contains(input.Filter)));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                switch (input.Sorting.ToLower())
                {
                    case "projectname":
                        query = query.OrderBy(p => p.ProjectName);
                        break;
                    case "projectname desc":
                        query = query.OrderByDescending(p => p.ProjectName);
                        break;
                    case "createddate":
                        query = query.OrderBy(p => p.CreatedDate);
                        break;
                    case "createddate desc":
                        query = query.OrderByDescending(p => p.CreatedDate);
                        break;
                    case "projectstatus":
                        query = query.OrderBy(p => p.ProjectStatus);
                        break;
                    case "projectstatus desc":
                        query = query.OrderByDescending(p => p.ProjectStatus);
                        break;
                    default:
                        query = query.OrderByDescending(p => p.UpdatedDate);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(p => p.UpdatedDate);
            }

            var totalCount = await query.CountAsync();

            var projects = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .Select(p => new ProjectListDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    ProjectStatus = p.ProjectStatus,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate,
                    IsActive = p.IsActive,
                    RelatedSaleOrdersCount = p.SaleOrders.Count()
                })
                .ToListAsync();

            return new PagedResultDto<ProjectListDto>(totalCount, projects);
        }

        public async Task<ProjectDto> GetAsync(int id, int companyId)
        {
            var project = await _context.Projects
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

            if (project == null)
                throw new ArgumentException($"Project with ID {id} not found or access denied.");

            var projectDto = _mapper.Map<ProjectDto>(project);
            projectDto.CompanyName = project.Company?.Name;

            return projectDto;
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto input, int companyId)
        {
            var project = _mapper.Map<Project>(input);
            project.CompanyId = companyId;
            project.CreatedDate = DateTime.UtcNow;
            project.UpdatedDate = DateTime.UtcNow;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return await GetAsync(project.Id, companyId);
        }

        public async Task<ProjectDto> UpdateAsync(UpdateProjectDto input, int companyId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == input.Id && p.CompanyId == companyId);

            if (project == null)
                throw new ArgumentException($"Project with ID {input.Id} not found or access denied.");

            _mapper.Map(input, project);
            project.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetAsync(project.Id, companyId);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var project = await _context.Projects
                .Include(p => p.SaleOrders)
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

            if (project == null)
                throw new ArgumentException($"Project with ID {id} not found or access denied.");

            // Check if project has related sale orders
            if (project.SaleOrders.Any())
            {
                // Soft delete by setting IsActive to false
                project.IsActive = false;
                project.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                // Hard delete if no related data
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ProjectDto>> GetProjectsByCompanyAsync(int companyId)
        {
            var projects = await _context.Projects
                .Where(p => p.CompanyId == companyId && p.IsActive)
                .OrderBy(p => p.ProjectName)
                .ToListAsync();

            return _mapper.Map<List<ProjectDto>>(projects);
        }
    }
}