using LU2_API_Herkansing.Authentication;
using LU2_API_Herkansing.Interfaces;
using LU2_API_Herkansing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LU2_API_Herkansing.Controllers
{
	[ApiController]
	[Authorize]
	[Route("objects")]
	public class ObjectController(IEnvironmentRepository environmentRepository, IObjectRepository objectRepository, IAuthenticationService authenticationService) : ControllerBase
	{
		private readonly IEnvironmentRepository _environmentRepository = environmentRepository;
		private readonly IObjectRepository _objectRepository = objectRepository;
		private readonly IAuthenticationService _authenticationService = authenticationService;

		[Authorize]
		[HttpPost]
		public ActionResult<Guid> CreateObject([FromBody] Object2D newObject) {
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			Environment2D? objectEnvironment = _environmentRepository.GetEnvironmentById(newObject.EnvironmentID);
			if (objectEnvironment == null || objectEnvironment.UserID != currentUserId) return NotFound("Environment could not be found.");

			Guid newObjectId = Guid.NewGuid();
			newObject.ID = newObjectId;

			_objectRepository.CreateObject(newObject);

			return Ok(newObjectId);
		}

		[Authorize]
		[HttpGet("environment={environmentId:Guid}")]
		public ActionResult<Object2D> GetObjects(Guid environmentId) {
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			Environment2D? environment = _environmentRepository.GetEnvironmentById(environmentId);
			if (environment == null || environment.UserID != currentUserId) return NotFound("Environment could not be found.");

			IEnumerable<Object2D> objects = _objectRepository.GetEnvironmentObjects(environmentId);
			return Ok(objects);
		}

		[Authorize]
		[HttpPut]
		public ActionResult UpdateObject([FromBody] Object2D updatedObject) {
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			Environment2D? environment = _environmentRepository.GetEnvironmentById(updatedObject.EnvironmentID);
			if (environment == null || environment.UserID != currentUserId) return NotFound("Environment could not be found.");

			_objectRepository.UpdateObject(updatedObject);

			return Ok();
		}

		[Authorize]
		[HttpDelete("object={objectId:Guid}")]
		public ActionResult DeleteObject(Guid objectId) {
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			Object2D? existingObject = _objectRepository.GetObject(objectId);
			if (existingObject == null) return NotFound("Object could not be found.");

			Environment2D? environment = _environmentRepository.GetEnvironmentById(existingObject.EnvironmentID);
			if (environment == null || environment.UserID != currentUserId) return NotFound("Environment could not be found.");

			_objectRepository.DeleteObject(objectId);

			return Ok();
		}
	}
}
