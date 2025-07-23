using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IProjectService
    {
        Task<PagedResultDto<ProjectListDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<ProjectDto> GetAsync(int id, int companyId);
        Task<ProjectDto> CreateAsync(CreateProjectDto input, int companyId);
        Task<ProjectDto> UpdateAsync(UpdateProjectDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
        Task<List<ProjectDto>> GetProjectsByCompanyAsync(int companyId);
    }
}