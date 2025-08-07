using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

	}
}
