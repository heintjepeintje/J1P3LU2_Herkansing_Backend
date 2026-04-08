using LU2_API_Herkansing.Authentication;
using LU2_API_Herkansing.Interfaces;
using LU2_API_Herkansing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LU2_API_Herkansing.Controllers
{
	[ApiController]
	[Authorize]
	[Route("environments")]
	public class EnvironmentController(IEnvironmentRepository environmentRepository, IObjectRepository objectRepository, IAuthenticationService auth) : ControllerBase
	{
		private readonly IEnvironmentRepository _environmentRepository = environmentRepository;
		private readonly IObjectRepository _objectRepository = objectRepository;
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

			bool result = _environmentRepository.CreateEnvironment(newEnvironment);

			return result ? Ok(newEnvironment.ID) : NotFound("An unknown error occurred.");
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
		[HttpPut]
		public ActionResult UpdateEnvironment(Environment2D environment) {
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			Environment2D? foundEnvironment = _environmentRepository.GetEnvironmentById(environment.ID);
			if (foundEnvironment == null || foundEnvironment?.UserID != currentUserId) return NotFound("Environment could not be found.");

			if (environment.Name != null) {
				if (environment.Name.Length == 0 || environment.Name.Length > 25) return BadRequest("Environment name must be between 1 and 25 characters long.");
			}

			if (environment.Width != 0) {
				if (environment.Width < 20 || environment.Width > 200) return BadRequest("Width must be between 20 and 200 units.");
			}

			if (environment.Height != 0) {
				if (environment.Height < 10 || environment.Height > 100) return BadRequest("Height must be between 10 and 100 units.");
			}

			bool success = _environmentRepository.UpdateEnvironment(environment);

			return success ? Ok() : NotFound("An unknown error occurred.");
		}

		[Authorize]
		[HttpDelete]
		public ActionResult DeleteEnvironment(Guid environmentId)
		{
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			Environment2D? environment = _environmentRepository.GetEnvironmentById(environmentId);
			if (environment == null || environment?.UserID != currentUserId) return NotFound("Environment could not be found.");

			IEnumerable<Object2D> environmentObjects = _objectRepository.GetEnvironmentObjects(environmentId);
			foreach (Object2D obj in environmentObjects) {
				_objectRepository.DeleteObject(obj.ID);
			}

			bool success = _environmentRepository.DeleteEnvironment(environmentId);

			return success ? Ok() : NotFound("An unknown error occurred.");
		}

	}
}
