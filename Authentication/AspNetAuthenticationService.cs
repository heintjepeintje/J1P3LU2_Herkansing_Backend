using System.Security.Claims;

namespace LU2_API_Herkansing.Authentication
{
	public class AspNetAuthenticationService(IHttpContextAccessor httpContextAccessor) : IAuthenticationService
	{
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

		public Guid? GetCurrentUserId()
		{
			if (!Guid.TryParse(
				_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, 
				out Guid result))
				return null;
			return result;
		}
	}
}
