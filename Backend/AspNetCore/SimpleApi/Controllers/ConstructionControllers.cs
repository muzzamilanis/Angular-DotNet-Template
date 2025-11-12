using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [Route("api/construction/projects")]
    [ApiController]
    [Authorize]
    public class ConstructionProjectsController : BaseController
    {
        private readonly ConstructionProjectService _projectService;

        public ConstructionProjectsController(ConstructionProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ConstructionProjectDto>>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _projectService.GetAllAsync(input, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConstructionProjectDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _projectService.GetAsync(id, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ConstructionProjectDto>> Create([FromBody] CreateConstructionProjectDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var result = await _projectService.CreateAsync(input, companyId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ConstructionProjectDto>> Update(int id, [FromBody] UpdateConstructionProjectDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != input.Id)
                    return BadRequest("ID mismatch");

                var companyId = GetCurrentUserCompanyId();
                var result = await _projectService.UpdateAsync(input, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                await _projectService.DeleteAsync(id, companyId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<ConstructionDashboardDto>> GetDashboard()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _projectService.GetDashboardDataAsync(companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/profit-loss")]
        public async Task<ActionResult<ProjectProfitLossDto>> GetProfitLoss(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _projectService.GetProjectProfitLossAsync(id, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    [Route("api/construction/clients")]
    [ApiController]
    [Authorize]
    public class ProjectClientsController : BaseController
    {
        private readonly ProjectClientService _clientService;

        public ProjectClientsController(ProjectClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("by-project/{projectId}")]
        public async Task<ActionResult<List<ProjectClientDto>>> GetByProject(int projectId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _clientService.GetByProjectAsync(projectId, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectClientDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _clientService.GetAsync(id, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProjectClientDto>> Create([FromBody] CreateProjectClientDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var result = await _clientService.CreateAsync(input, companyId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectClientDto>> Update(int id, [FromBody] UpdateProjectClientDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != input.Id)
                    return BadRequest("ID mismatch");

                var companyId = GetCurrentUserCompanyId();
                var result = await _clientService.UpdateAsync(input, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                await _clientService.DeleteAsync(id, companyId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    [Route("api/construction/investors")]
    [ApiController]
    [Authorize]
    public class ProjectInvestorsController : BaseController
    {
        private readonly ProjectInvestorService _investorService;

        public ProjectInvestorsController(ProjectInvestorService investorService)
        {
            _investorService = investorService;
        }

        [HttpGet("by-project/{projectId}")]
        public async Task<ActionResult<List<ProjectInvestorDto>>> GetByProject(int projectId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _investorService.GetByProjectAsync(projectId, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectInvestorDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _investorService.GetAsync(id, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProjectInvestorDto>> Create([FromBody] CreateProjectInvestorDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var result = await _investorService.CreateAsync(input, companyId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectInvestorDto>> Update(int id, [FromBody] UpdateProjectInvestorDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != input.Id)
                    return BadRequest("ID mismatch");

                var companyId = GetCurrentUserCompanyId();
                var result = await _investorService.UpdateAsync(input, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                await _investorService.DeleteAsync(id, companyId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    [Route("api/construction/payments")]
    [ApiController]
    [Authorize]
    public class ProjectPaymentsController : BaseController
    {
        private readonly ProjectPaymentService _paymentService;

        public ProjectPaymentsController(ProjectPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("by-project/{projectId}")]
        public async Task<ActionResult<List<ProjectPaymentDto>>> GetByProject(int projectId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _paymentService.GetByProjectAsync(projectId, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectPaymentDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _paymentService.GetAsync(id, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProjectPaymentDto>> Create([FromBody] CreateProjectPaymentDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var userId = GetCurrentUserId();
                var result = await _paymentService.CreateAsync(input, companyId, userId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    [Route("api/construction/expenses")]
    [ApiController]
    [Authorize]
    public class ProjectExpensesController : BaseController
    {
        private readonly ProjectExpenseService _expenseService;

        public ProjectExpensesController(ProjectExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet("by-project/{projectId}")]
        public async Task<ActionResult<List<ProjectExpenseDto>>> GetByProject(int projectId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _expenseService.GetByProjectAsync(projectId, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectExpenseDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _expenseService.GetAsync(id, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProjectExpenseDto>> Create([FromBody] CreateProjectExpenseDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var userId = GetCurrentUserId();
                var result = await _expenseService.CreateAsync(input, companyId, userId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    [Route("api/construction/ledgers")]
    [ApiController]
    [Authorize]
    public class ConstructionLedgersController : BaseController
    {
        private readonly ConstructionLedgerService _ledgerService;

        public ConstructionLedgersController(ConstructionLedgerService ledgerService)
        {
            _ledgerService = ledgerService;
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<List<ClientLedgerEntryDto>>> GetClientLedger(int clientId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _ledgerService.GetClientLedgerAsync(clientId, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("investor/{investorId}")]
        public async Task<ActionResult<List<InvestorLedgerEntryDto>>> GetInvestorLedger(int investorId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _ledgerService.GetInvestorLedgerAsync(investorId, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<ProjectLedgerEntryDto>>> GetProjectLedger(int projectId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _ledgerService.GetProjectLedgerAsync(projectId, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
