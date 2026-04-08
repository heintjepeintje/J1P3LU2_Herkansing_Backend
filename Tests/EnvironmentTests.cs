using LU2_API_Herkansing.Authentication;
using LU2_API_Herkansing.Controllers;
using LU2_API_Herkansing.Interfaces;
using LU2_API_Herkansing.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace APITests
{
	[TestClass]
	public sealed class EnvironmentTests
	{
		private readonly Mock<IEnvironmentRepository> _mockEnvironmentRepository;
		private readonly Mock<IAuthenticationService> _mockAuthenticationService;

		private EnvironmentController _environmentController;

		private Guid _currentUserId;

		private List<Environment2D> _environments;

		public EnvironmentTests() {
			_mockEnvironmentRepository = new();
			_mockAuthenticationService = new();
			
			_currentUserId = Guid.NewGuid();
			_mockAuthenticationService.Setup(auth => auth.GetCurrentUserId()).Returns(_currentUserId);

			_environments = new List<Environment2D>();

			_mockEnvironmentRepository.Setup(repository => repository.CreateEnvironment(It.IsAny<Environment2D>()))
				.Returns((Environment2D environment) =>
				{
					_environments.Add(environment);
					return true;
				});

			_mockEnvironmentRepository.Setup(repository => repository.GetEnvironmentsByUser(It.IsAny<Guid>()))
				.Returns((Guid userId) => _environments.Where(environment => environment.UserID == userId));

			_mockEnvironmentRepository.Setup(repository => repository.GetEnvironmentById(It.IsAny<Guid>()))
				.Returns((Guid environmentId) => {
					IEnumerable<Environment2D> foundEnvironments = _environments.Where(environment => environment.ID == environmentId);
					return foundEnvironments.FirstOrDefault();
				});

			_mockEnvironmentRepository.Setup(repository => repository.DeleteEnvironment(It.IsAny<Guid>()))
				.Returns((Guid environmentId) =>
				{
					int elementsRemoved = _environments.RemoveAll(environment => environment.ID == environmentId);
					return elementsRemoved > 0;
				});

			_environmentController = new(_mockEnvironmentRepository.Object, _mockAuthenticationService.Object);
		}

		[TestMethod("Create valid environment.")]
		public void CreateEnvironment_ReturnsOk()
		{
			Environment2D environment = new()
			{
				ID = Guid.NewGuid(),
				UserID = _currentUserId,
				Name = "Testing World",
				Width = 100,
				Height = 100
			};

			_mockEnvironmentRepository.Setup(repository => repository.CreateEnvironment(It.IsAny<Environment2D>())).Returns(true);
			ActionResult<Guid> actionResult = _environmentController.CreateEnvironment(environment);

			Assert.IsInstanceOfType<OkObjectResult>(actionResult.Result);

			_environments.Clear();
		}

		[TestMethod("Create invalid environment.")]
		public void CreateInvalidEnvironment_ReturnsBadRequest()
		{
			Environment2D environment = new()
			{
				ID = Guid.NewGuid(),
				UserID = Guid.Empty, // UserID wordt hier niet gebruikt. 
				Name = "Testing World",
				Width = 1000,
				Height = 1000
			};

			_mockEnvironmentRepository.Setup(repository => repository.CreateEnvironment(It.IsAny<Environment2D>())).Returns(true);

			ActionResult<Guid> actionResult = _environmentController.CreateEnvironment(environment);

			Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult.Result);

			_environments.Clear();
		}

		[TestMethod("Get environment with valid id.")]
		public void GetEnvironmentWithValidId_ReturnsOk() {
			Guid newEnvironmentId = Guid.NewGuid();

			Environment2D newEnvironment = new() {
				ID = newEnvironmentId,
				UserID = _currentUserId,
				Name = "Testing World 0",
				Width = 10,
				Height = 50
			};

			_environments.Add(newEnvironment);

			ActionResult<Environment2D> actionResult = _environmentController.GetEnvironment(newEnvironmentId);

			Assert.IsInstanceOfType<OkObjectResult>(actionResult.Result);

			_environments.Clear();
		}

		[TestMethod("Get environment with invalid id.")]
		public void GetEnvironmentWithInValidId_ReturnsNotFound()
		{
			ActionResult<Environment2D> actionResult = _environmentController.GetEnvironment(Guid.NewGuid());

			Assert.IsInstanceOfType<NotFoundObjectResult>(actionResult.Result);
		}

		[TestMethod("Get environment with non-owning user id.")]
		public void GetEnvironmentWithNonOwningUserId_ReturnsNotFound()
		{
			Guid newEnvironmentId = Guid.NewGuid();

			Environment2D newEnvironment = new()
			{
				ID = newEnvironmentId,
				UserID = Guid.NewGuid(),
				Name = "Testing World 0",
				Width = 10,
				Height = 50
			};

			_environments.Add(newEnvironment);

			ActionResult<Environment2D> actionResult = _environmentController.GetEnvironment(newEnvironmentId);

			Assert.IsInstanceOfType<NotFoundObjectResult>(actionResult.Result);

			_environments.Clear();
		}

		[TestMethod("Update environment with invalid environment id.")]
		public void UpdateEnvironmentWithInvalidId_ReturnsNotFound() {
			
		}

		[TestMethod("Delete environment with invalid id.")]
		public void DeleteEnvironmentWithInvalidId_ReturnsNotFound()
		{
			ActionResult actionResult = _environmentController.DeleteEnvironment(Guid.NewGuid());

			Assert.IsInstanceOfType<NotFoundObjectResult>(actionResult);
		}

		[TestMethod("Delete environment with non-owning user id.")]
		public void DeleteEnvironmentWithNonOwningUserId_ReturnsNotFound()
		{
			Guid newEnvironmentId = Guid.NewGuid();

			Environment2D newEnvironment = new()
			{
				ID = newEnvironmentId,
				UserID = Guid.NewGuid(),
				Name = "Testing World 0",
				Width = 10,
				Height = 50
			};

			_environments.Add(newEnvironment);

			ActionResult actionResult = _environmentController.DeleteEnvironment(Guid.NewGuid());

			Assert.IsInstanceOfType<NotFoundObjectResult>(actionResult);

			_environments.Clear();
		}

		[TestMethod("Delete environment with valid id.")]
		public void DeleteEnvironmentWithValidId_ReturnsOk()
		{
			Guid newEnvironmentId = Guid.NewGuid();

			Environment2D newEnvironment = new()
			{
				ID = newEnvironmentId,
				UserID = _currentUserId,
				Name = "Testing World 0",
				Width = 10,
				Height = 50
			};

			_environments.Add(newEnvironment);

			ActionResult actionResult = _environmentController.DeleteEnvironment(newEnvironmentId);

			Assert.IsInstanceOfType<OkResult>(actionResult);

			_environments.Clear();
		}
	}
}
