using LU2_API_Herkansing.Authentication;
using LU2_API_Herkansing.Interfaces;
using LU2_API_Herkansing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LU2_API_Herkansing.Controllers
{
	[ApiController]
	[Authorize]
	[Route("environments")]
	public class EnvironmentController(IEnvironmentRepository environmentRepository, IAuthenticationService auth) : ControllerBase
	{
		private readonly IEnvironmentRepository _environmentRepository = environmentRepository;
		private readonly IAuthenticationService _authenticationService = auth;

		[Authorize]
		[HttpPost]
		public ActionResult<Guid> CreateEnvironment(Environment2D newEnvironment)
		{
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			if (newEnvironment.Name == null) return BadRequest("Environment must have a name.");
			if (newEnvironment.Name.Length == 0 || newEnvironment.Name.Length > 25) return BadRequest("Environment name must be between 1 and 25 characters long.");
			if (newEnvironment.Width < 20 || newEnvironment.Width > 200) return BadRequest("Width must be between 20 and 200 units.");
			if (newEnvironment.Height < 10 || newEnvironment.Height > 100) return BadRequest("Height must be between 10 and 100 units.");

			IEnumerable<Environment2D> userEnvironments = _environmentRepository.GetEnvironmentsByUser(currentUserId.Value);
			if (userEnvironments.Count() >= 5) return BadRequest("You cannot have more than 5 environments.");
			if (userEnvironments.Any(environment => environment.Name == newEnvironment.Name)) return BadRequest("Environment name must be unique.");

			Guid newEnvironmentId = Guid.NewGuid();
			newEnvironment.ID = newEnvironmentId;
			newEnvironment.UserID = currentUserId.Value;

			_environmentRepository.CreateEnvironment(newEnvironment);

			return Ok(newEnvironment.ID);
		}

		[Authorize]
		[HttpGet]
		public ActionResult<Environment2D> GetEnvironment(Guid? environmentId)
		{
			
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			if (environmentId.HasValue) {
				Environment2D? environment = _environmentRepository.GetEnvironmentById(environmentId.Value);

				// We dont give a more specific response message for security reasons.
				if (environment == null || environment.UserID != currentUserId) return NotFound("Environment could not be found.");
	
				return Ok(environment);
			} else {
				IEnumerable<Environment2D> environments = _environmentRepository.GetEnvironmentsByUser(currentUserId.Value);
				return Ok(environments);
			}
		}

		[Authorize]
		[HttpDelete]
		public ActionResult DeleteEnvironment(Guid environmentId)
		{
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			Environment2D? environment = _environmentRepository.GetEnvironmentById(environmentId);
			if (environment == null) return NotFound("Environment is null.");
			if (environment == null || environment?.UserID != currentUserId) return NotFound("Environment could not be found.");

			_environmentRepository.DeleteEnvironment(environmentId);

			return Ok();
		}

	}
}
