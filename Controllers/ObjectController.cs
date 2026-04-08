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
	[Route("objects")]
	public class ObjectController(IEnvironmentRepository environmentRepository, IObjectRepository objectRepository, IAuthenticationService authenticationService) : ControllerBase
	{
		private readonly IEnvironmentRepository _environmentRepository = environmentRepository;
		private readonly IObjectRepository _objectRepository = objectRepository;
		private readonly IAuthenticationService _authenticationService = authenticationService;

		[Authorize]
		[HttpPost]
		public ActionResult CreateObject(IEnumerable<Object2D> newObjects) {
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			if (!newObjects.Any()) return BadRequest("Object count must be greater than 1.");

			bool success = true;

			foreach (Object2D newObject in newObjects) {
				Environment2D? objectEnvironment = _environmentRepository.GetEnvironmentById(newObject.EnvironmentID);
				if (objectEnvironment == null || objectEnvironment.UserID != currentUserId) return NotFound("Environment could not be found.");

				Guid newObjectId = Guid.NewGuid();
				newObject.ID = newObjectId;

				if (!_objectRepository.CreateObject(newObject)) {
					success = false;
					break;
				}
			}

			return success ? Ok() : NotFound("An unknown error occurred.");
		}

		[Authorize]
		[HttpGet]
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
		public ActionResult UpdateObject(Object2D updatedObject) {
			Guid? currentUserId = _authenticationService.GetCurrentUserId();
			if (!currentUserId.HasValue) return Unauthorized();

			Environment2D? environment = _environmentRepository.GetEnvironmentById(updatedObject.EnvironmentID);
			if (environment == null || environment.UserID != currentUserId) return NotFound("Environment could not be found.");

			IEnumerable<Object2D> objects = _objectRepository.GetEnvironmentObjects(updatedObject.EnvironmentID);
			if (!objects.Any(obj => obj.ID == updatedObject.ID)) return NotFound("Object could not be found.");

			bool success = _objectRepository.UpdateObject(updatedObject);

			return success ? Ok() : NotFound("An unknown error occurred.");
		}

		[Authorize]
		[HttpDelete]
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
