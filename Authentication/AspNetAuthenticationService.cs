using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace LU2_API_Herkansing.Authentication
{
	public class AspNetAuthenticationService(IHttpContextAccessor httpContextAccessor) : IAuthenticationService
	{
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

		public Guid? GetCurrentUserId()
		{
			string? userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId)) return null;
			return Guid.Parse(userId);
		}
	}
}
