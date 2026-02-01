using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface ISaleOrderService
    {
        Task<PagedResultDto<SaleOrderDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<SaleOrderDto> GetAsync(int id, int companyId);
        Task<SaleOrderDto> CreateAsync(CreateSaleOrderDto input, int companyId);
        Task<SaleOrderDto> UpdateAsync(UpdateSaleOrderDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
        Task<List<SaleOrder>> GetDeadlineApproachingOrdersAsync();
        // Add to ISaleOrderService
        Task<WriterWorkloadDto> GetWriterWorkloadAsync(int writerId, int companyId);
        bool IsDeadlineApproaching(SaleOrder order);
        
        // Writer Portal methods
        Task<List<WriterAssignmentDto>> GetWriterAssignmentsAsync(int writerId);
        Task SubmitWriterWorkAsync(WriterSubmissionDto submission, IFormFile? submittedFile, int writerId);
        
        // Team Lead methods
        Task<List<TeamLeadOrderDto>> GetTeamLeadOrdersAsync(int companyId);
        Task AssignOrderToWriterAsync(int saleOrderId, int writerId, int companyId);
        Task UpdateOrderAssignmentAsync(int saleOrderId, int writerId, int companyId);
        
        // Order Instructions methods
        Task<List<OrderInstructionDto>> GetOrderInstructionsAsync(int saleOrderId, int companyId);
        Task<OrderInstructionDto> AddOrderInstructionAsync(CreateOrderInstructionDto instruction, int senderId, bool isFromTeamLead, int companyId);
        Task<OrderInstructionDto> AddOrderInstructionWithAttachmentAsync(CreateOrderInstructionDto instruction, int senderId, bool isFromTeamLead, int companyId, IFormFile? attachmentFile);
        Task UpdateOrderDetailsAsync(UpdateOrderDetailsDto updateDto, int companyId);
    }

    public class SaleOrderService : ISaleOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SaleOrderService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<SaleOrderDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.SaleOrders
                .Where(so => so.CompanyId == companyId)
                .Include(so => so.Items)
                .ThenInclude(i => i.Product)
                .Include(so => so.Project)
                .Include(so => so.ServiceType)
                .Include(so => so.Company)
                .AsQueryable();

            // Apply filtering
            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(so => so.OrderNumber.Contains(input.Filter) || 
                                         (so.CustomerName != null && so.CustomerName.Contains(input.Filter)) ||
                                         (so.ClientName != null && so.ClientName.Contains(input.Filter)));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                switch (input.Sorting.ToLower())
                {
                    case "ordernumber":
                        query = query.OrderBy(so => so.OrderNumber);
                        break;
                    case "ordernumber desc":
                        query = query.OrderByDescending(so => so.OrderNumber);
                        break;
                    case "orderdate":
                        query = query.OrderBy(so => so.OrderDate);
                        break;
                    case "orderdate desc":
                        query = query.OrderByDescending(so => so.OrderDate);
                        break;
                    case "customername":
                        query = query.OrderBy(so => so.CustomerName);
                        break;
                    case "customername desc":
                        query = query.OrderByDescending(so => so.CustomerName);
                        break;
                    case "totalamount":
                        query = query.OrderBy(so => so.TotalAmount);
                        break;
                    case "totalamount desc":
                        query = query.OrderByDescending(so => so.TotalAmount);
                        break;
                    default:
                        query = query.OrderByDescending(so => so.OrderDate);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(so => so.OrderDate);
            }
            
            var totalCount = await query.CountAsync();
            var saleOrders = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            
            var dtos = _mapper.Map<List<SaleOrderDto>>(saleOrders);
            
            return new PagedResultDto<SaleOrderDto>(totalCount, dtos);
        }

        public async Task<SaleOrderDto> GetAsync(int id, int companyId)
        {
            var saleOrder = await _context.SaleOrders
                .Where(so => so.Id == id && so.CompanyId == companyId)
                .Include(so => so.Items)
                .ThenInclude(i => i.Product)
                .Include(so => so.Project)
                .Include(so => so.ServiceType)
                .FirstOrDefaultAsync();

            if (saleOrder == null)
            {
                throw new ArgumentException($"Sale order with ID {id} not found or access denied.");
            }

            var dto = _mapper.Map<SaleOrderDto>(saleOrder);
            
            // Map product names for items
            foreach (var item in dto.Items)
            {
                var saleOrderItem = saleOrder.Items.First(i => i.Id == item.Id);
                item.ProductName = saleOrderItem.Product?.Name;
            }

            return dto;
        }

        public async Task<SaleOrderDto> CreateAsync(CreateSaleOrderDto input, int companyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var saleOrder = _mapper.Map<SaleOrder>(input);
                saleOrder.CompanyId = companyId;

                // Validate Project belongs to the same company if specified
                if (input.ProjectId.HasValue)
                {
                    var project = await _context.Projects
                        .FirstOrDefaultAsync(p => p.Id == input.ProjectId.Value && p.CompanyId == companyId);
                    if (project == null)
                    {
                        throw new ArgumentException("Invalid project specified or access denied.");
                    }
                }
                
                // Calculate total amount
                decimal totalAmount = 0;
                foreach (var itemDto in input.Items)
                {
                    totalAmount += itemDto.Quantity * itemDto.Price;
                }
                saleOrder.TotalAmount = totalAmount;

                // Clear the items mapped by AutoMapper since we'll handle them manually
                saleOrder.Items.Clear();

                _context.SaleOrders.Add(saleOrder);
                await _context.SaveChangesAsync();

                // Create items after the sale order is saved (to get the ID)
                foreach (var itemDto in input.Items)
                {
                    // Validate Product belongs to the same company
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == itemDto.ProductId && p.CompanyId == companyId);
                    if (product == null)
                    {
                        throw new ArgumentException($"Invalid product specified (ID: {itemDto.ProductId}) or access denied.");
                    }

                    var item = _mapper.Map<SaleOrderItem>(itemDto);
                    item.SaleOrderId = saleOrder.Id;
                    _context.SaleOrderItems.Add(item);
                }

                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                return await GetAsync(saleOrder.Id, companyId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<SaleOrderDto> UpdateAsync(UpdateSaleOrderDto input, int companyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var saleOrder = await _context.SaleOrders
                    .Where(so => so.Id == input.Id && so.CompanyId == companyId)
                    .Include(so => so.Items)
                    .FirstOrDefaultAsync();
                    
                if (saleOrder == null)
                {
                    throw new ArgumentException($"Sale order with ID {input.Id} not found or access denied.");
                }

                // Validate Project belongs to the same company if specified
                if (input.ProjectId.HasValue)
                {
                    var project = await _context.Projects
                        .FirstOrDefaultAsync(p => p.Id == input.ProjectId.Value && p.CompanyId == companyId);
                    if (project == null)
                    {
                        throw new ArgumentException("Invalid project specified or access denied.");
                    }
                }
                
                // Update sale order properties
                _mapper.Map(input, saleOrder);

                // Delete existing items
                _context.SaleOrderItems.RemoveRange(saleOrder.Items);

                // Calculate total amount and create new items
                decimal totalAmount = 0;
                var newItems = new List<SaleOrderItem>();
                foreach (var itemDto in input.Items)
                {
                    // Validate Product belongs to the same company
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == itemDto.ProductId && p.CompanyId == companyId);
                    if (product == null)
                    {
                        throw new ArgumentException($"Invalid product specified (ID: {itemDto.ProductId}) or access denied.");
                    }

                    totalAmount += itemDto.Quantity * itemDto.Price;
                    var item = _mapper.Map<SaleOrderItem>(itemDto);
                    item.SaleOrderId = saleOrder.Id;
                    newItems.Add(item);
                }
                saleOrder.TotalAmount = totalAmount;
                saleOrder.Items = newItems;

                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                return await GetAsync(saleOrder.Id, companyId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var saleOrder = await _context.SaleOrders
                    .Where(so => so.Id == id && so.CompanyId == companyId)
                    .Include(so => so.Items)
                    .FirstOrDefaultAsync();
                    
                if (saleOrder == null)
                {
                    throw new ArgumentException($"Sale order with ID {id} not found or access denied.");
                }
                
                _context.SaleOrders.Remove(saleOrder);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<SaleOrder>> GetDeadlineApproachingOrdersAsync()
        {
            var deadlineThreshold = DateTime.UtcNow.Date.AddDays(3);
            
            return await _context.SaleOrders
                .Where(so => so.Status == "InProgress" && 
                            so.Deadline.HasValue && 
                            so.Deadline.Value.ToUniversalTime() <= deadlineThreshold)
                .Include(so => so.Company)
                .ToListAsync();
        }

        public bool IsDeadlineApproaching(SaleOrder order)
        {
            if (!order.Deadline.HasValue || order.Status != "InProgress")
                return false;
                
            return order.Deadline.Value <= DateTime.Today.AddDays(3);
        }

        public async Task<List<WriterAssignmentDto>> GetWriterAssignmentsAsync(int writerId)
        {
            List<WriterAssignmentDto> assignments = new List<WriterAssignmentDto>();
            // Get the writer's name first
            var writer = await _context.Users.FindAsync(writerId);
            if (writer == null)
                throw new ArgumentException("Writer not found");
            if (writer.Role == UserRole.SuperAdmin)
            {
                assignments = await _context.SaleOrders
                .Select(so => new WriterAssignmentDto
                {
                    Id = so.Id,
                    OrderNumber = so.OrderNumber,
                    OrderDate = so.OrderDate,
                    ClientName = so.ClientName,
                    Deadline = so.Deadline,
                    Status = so.Status,
                    OrderDetails = so.OrderDetails,
                    WordsOrPages = so.WordsOrPages,
                    AcademicLevel = so.AcademicLevel,
                    AttachmentFilenames = so.AttachmentFilenames,
                    SubmissionStatus = so.SubmissionStatus,
                    SubmissionDate = so.SubmissionDate,
                    SubmittedFilename = so.SubmittedFilename
                })
                .ToListAsync();
            }
            else
            {
				assignments = await _context.SaleOrders
				.Where(so => so.AssignedTo == writer.Name)
				.Select(so => new WriterAssignmentDto
				{
					Id = so.Id,
					OrderNumber = so.OrderNumber,
					OrderDate = so.OrderDate,
					ClientName = so.ClientName,
					Deadline = so.Deadline,
					Status = so.Status,
					OrderDetails = so.OrderDetails,
					WordsOrPages = so.WordsOrPages,
					AcademicLevel = so.AcademicLevel,
					AttachmentFilenames = so.AttachmentFilenames,
					SubmissionStatus = so.SubmissionStatus,
					SubmissionDate = so.SubmissionDate,
					SubmittedFilename = so.SubmittedFilename
				})
				.ToListAsync();
			}
                

            return assignments;
        }

        public async Task SubmitWriterWorkAsync(WriterSubmissionDto submission, IFormFile? submittedFile, int writerId)
        {
            // Get the writer's name first
            var writer = await _context.Users.FindAsync(writerId);
            if (writer == null)
                throw new ArgumentException("Writer not found");

            var saleOrder = await _context.SaleOrders
                .FirstOrDefaultAsync(so => so.Id == submission.SaleOrderId && so.AssignedTo == writer.Name);

            if (saleOrder == null)
                throw new ArgumentException("Sale order not found or not assigned to this writer");

            // Handle file upload if provided
            string? submittedFilename = null;
            if (submittedFile != null && submittedFile.Length > 0)
            {
                // Create assets directory if it doesn't exist
                var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
                if (!Directory.Exists(assetsPath))
                    Directory.CreateDirectory(assetsPath);

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}_{submittedFile.FileName}";
                var filePath = Path.Combine(assetsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await submittedFile.CopyToAsync(stream);
                }

                submittedFilename = fileName;
            }

            // Update sale order with submission details
            saleOrder.SubmissionStatus = submission.SubmissionStatus;
            saleOrder.SubmissionDate = DateTime.UtcNow;
            saleOrder.SubmittedFilename = submittedFilename ?? submission.SubmittedFilename;

            // Update status based on submission
            if (submission.SubmissionStatus == "Completed")
            {
                saleOrder.Status = "workSubmittedByWriter";
            }

            await _context.SaveChangesAsync();
        }

        // Team Lead methods implementation
        public async Task<List<TeamLeadOrderDto>> GetTeamLeadOrdersAsync(int companyId)
        {
            // Get all orders for the company with basic info
            var allOrders = await _context.SaleOrders
                .Where(so => so.CompanyId == companyId)
                .OrderByDescending(so => so.OrderDate)
                .ToListAsync();

            var result = new List<TeamLeadOrderDto>();

            foreach (var order in allOrders)
            {
                var teamLeadOrder = new TeamLeadOrderDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    OrderDate = order.OrderDate,
                    ClientName = order.ClientName,
                    CustomerName = order.CustomerName,
                    Deadline = order.Deadline,
                    Status = order.Status,
                    OrderDetails = order.OrderDetails,
                    AcademicLevel = order.AcademicLevel,
                    Country = order.Country,
                    WordsOrPages = order.WordsOrPages,
                    SubmissionStatus = order.SubmissionStatus,
                    SubmissionDate = order.SubmissionDate,
                    AttachmentFilenames = order.AttachmentFilenames
                };

                // Get assigned writer info if order is assigned
                if (!string.IsNullOrEmpty(order.AssignedTo))
                {
                    // Try to parse as integer first (new format)
                    if (int.TryParse(order.AssignedTo, out int assignedUserId))
                    {
                        var assignedUser = await _context.Users
                            .FirstOrDefaultAsync(u => u.Id == assignedUserId && u.CompanyId == companyId);
                        
                        if (assignedUser != null)
                        {
                            teamLeadOrder.AssignedTo = assignedUser.Name;
                            teamLeadOrder.AssignedToUserId = assignedUser.Id;
                        }
                        else
                        {
                            // Handle case where user ID doesn't exist anymore (legacy data)
                            teamLeadOrder.AssignedTo = $"Unknown User (ID: {assignedUserId})";
                            teamLeadOrder.AssignedToUserId = null;
                        }
                    }
                    else
                    {
                        // Handle legacy case where AssignedTo contains name instead of ID
                        teamLeadOrder.AssignedTo = order.AssignedTo;
                        teamLeadOrder.AssignedToUserId = null;
                    }
                }

                result.Add(teamLeadOrder);
            }

            return result;
        }

        public async Task AssignOrderToWriterAsync(int saleOrderId, int writerId, int companyId)
        {
            // Validate sale order exists and belongs to company
            var saleOrder = await _context.SaleOrders
                .FirstOrDefaultAsync(so => so.Id == saleOrderId && so.CompanyId == companyId);

            if (saleOrder == null)
            {
                throw new ArgumentException($"Sale order with ID {saleOrderId} not found or access denied.");
            }

            // Validate writer exists, belongs to company, and has Writer role
            var writer = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == writerId && u.CompanyId == companyId && u.Role == UserRole.Writer && u.IsActive);

            if (writer == null)
            {
                throw new ArgumentException($"Writer with ID {writerId} not found, not active, or access denied.");
            }

            // Check if order can be reassigned (not completed)
            if (saleOrder.Status == "Completed")
            {
                throw new ArgumentException("Cannot assign completed orders.");
            }

            // For legacy data, allow reassignment even if already assigned to handle data migration
            // Only throw error if trying to assign to the same writer
            if (!string.IsNullOrEmpty(saleOrder.AssignedTo) && 
                int.TryParse(saleOrder.AssignedTo, out int currentWriterId) && 
                currentWriterId == writerId)
            {
                throw new ArgumentException("Order is already assigned to this writer.");
            }

            // Assign the order
            saleOrder.AssignedTo = writerId.ToString();
            saleOrder.Status = "InProgress"; // Set status to InProgress when assigned

            // Reset submission status if changing assignment after submission
            if (saleOrder.SubmissionStatus != null && saleOrder.SubmissionStatus != "")
            {
                saleOrder.SubmissionStatus = null;
                saleOrder.SubmissionDate = null;
                saleOrder.SubmittedFilename = null;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAssignmentAsync(int saleOrderId, int writerId, int companyId)
        {
            // Validate sale order exists and belongs to company
            var saleOrder = await _context.SaleOrders
                .FirstOrDefaultAsync(so => so.Id == saleOrderId && so.CompanyId == companyId);

            if (saleOrder == null)
            {
                throw new ArgumentException($"Sale order with ID {saleOrderId} not found or access denied.");
            }

            // Validate writer exists, belongs to company, and has Writer role
            var writer = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == writerId && u.CompanyId == companyId && u.Role == UserRole.Writer && u.IsActive);

            if (writer == null)
            {
                throw new ArgumentException($"Writer with ID {writerId} not found, not active, or access denied.");
            }

            // Check if order can be reassigned (not completed)
            if (saleOrder.Status == "Completed")
            {
                throw new ArgumentException("Cannot reassign completed orders.");
            }

            // Update the assignment
            saleOrder.AssignedTo = writerId.ToString();
            
            // Reset submission status if changing assignment after submission
            if (saleOrder.SubmissionStatus != null && saleOrder.SubmissionStatus != "")
            {
                saleOrder.SubmissionStatus = null;
                saleOrder.SubmissionDate = null;
                saleOrder.SubmittedFilename = null;
            }

            // Set status to InProgress when reassigned
            if (saleOrder.Status != "InProgress")
            {
                saleOrder.Status = "InProgress";
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderInstructionDto>> GetOrderInstructionsAsync(int saleOrderId, int companyId)
        {
            // Verify that the sale order belongs to the company
            var saleOrderExists = await _context.SaleOrders
                .AnyAsync(so => so.Id == saleOrderId && so.CompanyId == companyId);

            if (!saleOrderExists)
            {
                throw new ArgumentException($"Sale order with ID {saleOrderId} not found or access denied.");
            }

            var instructions = await _context.OrderInstructions
                .Include(oi => oi.Sender)
                .Where(oi => oi.SaleOrderId == saleOrderId)
                .OrderBy(oi => oi.CreatedAt)
                .Select(oi => new OrderInstructionDto
                {
                    Id = oi.Id,
                    SaleOrderId = oi.SaleOrderId,
                    SenderId = oi.SenderId,
                    SenderName = oi.Sender != null ? oi.Sender.Name : "Unknown",
                    Message = oi.Message,
                    CreatedAt = oi.CreatedAt,
                    IsFromTeamLead = oi.IsFromTeamLead,
                    AttachmentFilenames = oi.AttachmentFilenames
                })
                .ToListAsync();

            return instructions;
        }

        public async Task<OrderInstructionDto> AddOrderInstructionAsync(CreateOrderInstructionDto instruction, int senderId, bool isFromTeamLead, int companyId)
        {
            // Verify that the sale order belongs to the company
            var saleOrder = await _context.SaleOrders
                .FirstOrDefaultAsync(so => so.Id == instruction.SaleOrderId && so.CompanyId == companyId);

            if (saleOrder == null)
            {
                throw new ArgumentException($"Sale order with ID {instruction.SaleOrderId} not found or access denied.");
            }

            // Verify that the sender exists and belongs to the same company
            var sender = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == senderId && u.CompanyId == companyId && u.IsActive);

            if (sender == null)
            {
                throw new ArgumentException($"User with ID {senderId} not found or access denied.");
            }

            var orderInstruction = new OrderInstruction
            {
                SaleOrderId = instruction.SaleOrderId,
                SenderId = senderId,
                Message = instruction.Message,
                CreatedAt = DateTime.UtcNow,
                IsFromTeamLead = isFromTeamLead,
                AttachmentFilenames = instruction.AttachmentFilenames
            };

            _context.OrderInstructions.Add(orderInstruction);
            await _context.SaveChangesAsync();

            return new OrderInstructionDto
            {
                Id = orderInstruction.Id,
                SaleOrderId = orderInstruction.SaleOrderId,
                SenderId = orderInstruction.SenderId,
                SenderName = sender.Name,
                Message = orderInstruction.Message,
                CreatedAt = orderInstruction.CreatedAt,
                IsFromTeamLead = orderInstruction.IsFromTeamLead,
                AttachmentFilenames = orderInstruction.AttachmentFilenames
            };
        }

        public async Task<OrderInstructionDto> AddOrderInstructionWithAttachmentAsync(CreateOrderInstructionDto instruction, int senderId, bool isFromTeamLead, int companyId, IFormFile? attachmentFile)
        {
            // Verify that the sale order belongs to the company
            var saleOrder = await _context.SaleOrders
                .FirstOrDefaultAsync(so => so.Id == instruction.SaleOrderId && so.CompanyId == companyId);

            if (saleOrder == null)
            {
                throw new ArgumentException($"Sale order with ID {instruction.SaleOrderId} not found or access denied.");
            }

            // Verify that the sender exists and belongs to the same company
            var sender = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == senderId && u.CompanyId == companyId && u.IsActive);

            if (sender == null)
            {
                throw new ArgumentException($"User with ID {senderId} not found or access denied.");
            }

            // Handle file upload if provided
            string? attachmentFilename = null;
            if (attachmentFile != null && attachmentFile.Length > 0)
            {
                // Create assets directory if it doesn't exist
                var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
                if (!Directory.Exists(assetsPath))
                    Directory.CreateDirectory(assetsPath);

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}_{attachmentFile.FileName}";
                var filePath = Path.Combine(assetsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachmentFile.CopyToAsync(stream);
                }

                attachmentFilename = fileName;
            }

            var orderInstruction = new OrderInstruction
            {
                SaleOrderId = instruction.SaleOrderId,
                SenderId = senderId,
                Message = instruction.Message,
                CreatedAt = DateTime.UtcNow,
                IsFromTeamLead = isFromTeamLead,
                AttachmentFilenames = attachmentFilename ?? instruction.AttachmentFilenames
            };

            _context.OrderInstructions.Add(orderInstruction);
            await _context.SaveChangesAsync();

            return new OrderInstructionDto
            {
                Id = orderInstruction.Id,
                SaleOrderId = orderInstruction.SaleOrderId,
                SenderId = orderInstruction.SenderId,
                SenderName = sender.Name,
                Message = orderInstruction.Message,
                CreatedAt = orderInstruction.CreatedAt,
                IsFromTeamLead = orderInstruction.IsFromTeamLead,
                AttachmentFilenames = orderInstruction.AttachmentFilenames
            };
        }

        public async Task UpdateOrderDetailsAsync(UpdateOrderDetailsDto updateDto, int companyId)
        {
            var saleOrder = await _context.SaleOrders
                .FirstOrDefaultAsync(so => so.Id == updateDto.Id && so.CompanyId == companyId);

            if (saleOrder == null)
            {
                throw new ArgumentException($"Sale order with ID {updateDto.Id} not found or access denied.");
            }

            saleOrder.OrderDetails = updateDto.OrderDetails;
            await _context.SaveChangesAsync();
        }

        public async Task<WriterWorkloadDto> GetWriterWorkloadAsync(int writerId, int companyId)
        {
            // Verify that the writer exists and belongs to the company
            var writer = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == writerId && u.CompanyId == companyId && u.Role == UserRole.Writer);

            if (writer == null)
            {
                throw new ArgumentException($"Writer with ID {writerId} not found or access denied.");
            }

            // Get all assignments for the writer
            var assignments = await _context.SaleOrders
                .Where(so => so.AssignedTo == writerId.ToString())
                .ToListAsync();

            var totalAssignments = assignments.Count;
            var activeAssignments = assignments.Count(so => so.Status == "InProgress");
            var completedAssignments = assignments.Count(so => so.Status == "Completed");
            var pendingAssignments = assignments.Count(so => so.Status == "workSubmittedByWriter");

            // Get current assignments details
            var currentAssignments = assignments
                .Where(so => so.Status == "InProgress" || so.Status == "workSubmittedByWriter")
                .Select(so => new WriterAssignmentDto
                {
                    Id = so.Id,
                    OrderNumber = so.OrderNumber,
                    OrderDate = so.OrderDate,
                    ClientName = so.ClientName,
                    Deadline = so.Deadline,
                    Status = so.Status,
                    OrderDetails = so.OrderDetails,
                    WordsOrPages = so.WordsOrPages,
                    AcademicLevel = so.AcademicLevel,
                    AttachmentFilenames = so.AttachmentFilenames,
                    SubmissionStatus = so.SubmissionStatus,
                    SubmissionDate = so.SubmissionDate,
                    SubmittedFilename = so.SubmittedFilename
                })
                .ToList();

            // Define max capacity (can be made configurable)
            const int maxCapacity = 10;
            var hasCapacity = activeAssignments < maxCapacity;

            return new WriterWorkloadDto
            {
                WriterId = writerId,
                WriterName = writer.Name,
                TotalAssignments = totalAssignments,
                ActiveAssignments = activeAssignments,
                CompletedAssignments = completedAssignments,
                PendingAssignments = pendingAssignments,
                CurrentAssignments = currentAssignments,
                HasCapacity = hasCapacity,
                MaxCapacity = maxCapacity
            };
        }
    }
}
