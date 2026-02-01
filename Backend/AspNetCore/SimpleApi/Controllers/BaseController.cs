using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SimpleApi.Models;

namespace SimpleApi.Controllers
{
	public class BaseController : ControllerBase
	{
		internal int GetCurrentUserCompanyId()
		{
			var companyIdClaim = User.FindFirst("CompanyId")?.Value;
			if (int.TryParse(companyIdClaim, out int companyId))
			{
				return companyId;
			}
			throw new UnauthorizedAccessException("Unable to determine user's company.");
		}

		internal int GetCurrentUserId()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (int.TryParse(userIdClaim, out int userId))
			{
				return userId;
			}
			throw new UnauthorizedAccessException("Unable to determine user ID.");
		}

		internal bool GetCurrentUserIsSuperAdmin()
		{
			var isSuperAdminClaim = User.FindFirst("IsSuperAdmin")?.Value;
			return bool.TryParse(isSuperAdminClaim, out bool isSuperAdmin) && isSuperAdmin;
		}

		internal string? GetCurrentUserEmail()
		{
			return User.FindFirst(ClaimTypes.Email)?.Value;
		}

		internal UserRole GetCurrentUserRole()
		{
			var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (GetCurrentUserIsSuperAdmin() == true)
            {
                return UserRole.SuperAdmin;
            }
			else if (Enum.TryParse<UserRole>(roleClaim, out UserRole role))
			{
				return role;
			}
			throw new UnauthorizedAccessException("Unable to determine user role.");
		}

	}
}
