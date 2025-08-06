using Microsoft.AspNetCore.Mvc;

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

		internal bool GetCurrentUserIsSuperAdmin()
		{
			var isSuperAdminClaim = User.FindFirst("IsSuperAdmin")?.Value;
			return bool.TryParse(isSuperAdminClaim, out bool isSuperAdmin) && isSuperAdmin;
		}

	}
}
