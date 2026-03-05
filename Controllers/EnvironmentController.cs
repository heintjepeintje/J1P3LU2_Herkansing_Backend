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
	public class EnvironmentController(IEnvironmentRespository environmentRepository, IUserEnvironmentRepository userEnvironmentRepository, IAuthenticationService auth) : ControllerBase
	{
		private readonly IAuthenticationService _authenticationService = auth;
		private readonly IEnvironmentRespository _environmentRepository = environmentRepository;
		private readonly IUserEnvironmentRepository _userEnvironmentRepository = userEnvironmentRepository;

		[Authorize]
		[HttpPost]
		public ActionResult<Guid> CreateEnvironment(Environment2D newEnvironment)
		{
			Guid? currentUser = _authenticationService.GetCurrentUserId();
			if (currentUser == null) return Unauthorized();

			if (newEnvironment.Name == null || newEnvironment.Name.Length == 0) return BadRequest("Environment must have a name.");
			if (newEnvironment.Width == 0) return BadRequest("Width must be greater than 0.");
			if (newEnvironment.Height == 0) return BadRequest("Height must be greater than 0.");

			IEnumerable<Guid> userEnvironments = _userEnvironmentRepository.GetUserEnvironments(currentUser.Value);
			foreach (Guid userEnvironmentId in userEnvironments)
			{
				Environment2D? environment = _environmentRepository.GetEnvironmentById(userEnvironmentId);
				if (environment == null) continue;
				if (environment.Name.ToLower().Equals(newEnvironment.Name.ToLower())) return BadRequest("A world with that name already exists.");
			}

			Guid environmentId = Guid.NewGuid();
			newEnvironment.ID = environmentId;
			_environmentRepository.CreateEnvironment(newEnvironment);

			UserEnvironment userEnvironment = new(currentUser.Value, environmentId);
			_userEnvironmentRepository.CreateUserEnvironment(userEnvironment);

			return Ok(environmentId);
		}

		[Authorize]
		[HttpGet("id={id:Guid}")]
		public ActionResult<Environment2D> GetEnvironmentById(Guid id)
		{
			Guid? userId = _authenticationService.GetCurrentUserId();
			if (userId == null) return Unauthorized("Invalid user.");

			IEnumerable<UserEnvironment> environmentUsers = _userEnvironmentRepository.GetUserEnvironmentsByEnvironmentId(id);
			if (environmentUsers == null || !environmentUsers.Any()) return NotFound("Environment users could not be found.");
			if (!environmentUsers.Any(env => env.EnvironmentID == id)) return Unauthorized("Environment could not be found.");

			Environment2D? environment = _environmentRepository.GetEnvironmentById(id);
			if (environment == null) return NotFound("Environment could not be found.");

			return Ok(environment);
		}

		[Authorize]
		[HttpGet]
		public ActionResult<IEnumerable<Environment2D>?> GetEnvironments()
		{
			Guid? userId = _authenticationService.GetCurrentUserId();
			if (userId == null) return Unauthorized("Invalid user.");

			IEnumerable<UserEnvironment> userEnvironments = _userEnvironmentRepository.GetUserEnvironmentsByUserId(userId.Value);
			IEnumerable<Environment2D?> environments = userEnvironments.Select(env => _environmentRepository.GetEnvironmentById(env.EnvironmentID));

			return Ok(environments);
		}

		[Authorize]
		[HttpDelete("id={id:Guid}")]
		public ActionResult DeleteEnvironment([FromRoute] Guid id)
		{
			Guid? userId = _authenticationService.GetCurrentUserId();
			if (userId == null) return Unauthorized("Invalid user.");

			Guid? environmentOwner = _userEnvironmentRepository.GetEnvironmentOwner(id);
			if (environmentOwner != userId) return Unauthorized();



			return Ok();
		}

	}
}
