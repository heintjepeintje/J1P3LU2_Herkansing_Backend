using LU2_API_Herkansing.Authentication;
using LU2_API_Herkansing.Controllers;
using LU2_API_Herkansing.Interfaces;
using LU2_API_Herkansing.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace APITests
{
	[TestClass]
	public sealed class EnvironmentTests
	{
		private readonly Mock<IEnvironmentRepository> _mockEnvironmentRepository;
		private readonly Mock<IObjectRepository> _mockObjectRepository;
		private readonly Mock<IAuthenticationService> _mockAuthenticationService;

		private EnvironmentController _environmentController;

		private Guid _currentUserId;

		private List<Environment2D> _environments;

		public EnvironmentTests() {
			_mockEnvironmentRepository = new();
			_mockObjectRepository = new();
			_mockAuthenticationService = new();
			
			_currentUserId = Guid.NewGuid();
			_mockAuthenticationService.Setup(auth => auth.GetCurrentUserId()).Returns(_currentUserId);

			_environments = new List<Environment2D>();

			_mockObjectRepository.Setup(repository => repository.GetEnvironmentObjects(It.IsAny<Guid>()))
				.Returns((Guid id) => new List<Object2D>());

			_mockEnvironmentRepository.Setup(repository => repository.CreateEnvironment(It.IsAny<Environment2D>()))
				.Returns((Environment2D environment) =>
				{
					_environments.Add(environment);
					return true;
				});

			_mockEnvironmentRepository.Setup(repository => repository.GetEnvironmentsByUser(It.IsAny<Guid>()))
				.Returns((Guid userId) => _environments.Where(environment => environment.UserID == userId));

			_mockEnvironmentRepository.Setup(repository => repository.GetEnvironmentById(It.IsAny<Guid>()))
				.Returns((Guid environmentId) => _environments.Where(environment => environment.ID == environmentId).FirstOrDefault());

			_mockEnvironmentRepository.Setup(repository => repository.UpdateEnvironment(It.IsAny<Environment2D>()))
				.Returns((Environment2D environment) => {
					IEnumerable<Environment2D> foundEnvironments = _environments.Where(e => e.ID == environment.ID);
					Environment2D? foundEnvironment = foundEnvironments.FirstOrDefault();

					if (foundEnvironment == null) return false;

					foundEnvironment.Name = environment.Name;
					foundEnvironment.Width = environment.Width;
					foundEnvironment.Height = environment.Height;

					return true;
 				});

			_mockEnvironmentRepository.Setup(repository => repository.DeleteEnvironment(It.IsAny<Guid>()))
				.Returns((Guid environmentId) => _environments.RemoveAll(environment => environment.ID == environmentId) > 0);

			_environmentController = new(_mockEnvironmentRepository.Object, _mockObjectRepository.Object, _mockAuthenticationService.Object);
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

			ActionResult<Guid> actionResult = _environmentController.CreateEnvironment(environment);

			Assert.IsInstanceOfType<OkObjectResult>(actionResult.Result);

			_environments.Clear();
		}

		[TestMethod("Create environment with invalid size.")]
		public void CreateEnvironmentWithInvalidSize_ReturnsBadRequest()
		{
			Environment2D environment = new()
			{
				ID = Guid.NewGuid(),
				UserID = Guid.Empty, // UserID wordt hier niet gebruikt. 
				Name = "Testing World",
				Width = 1000,
				Height = 1000
			};

			ActionResult<Guid> actionResult = _environmentController.CreateEnvironment(environment);

			Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult.Result);

			_environments.Clear();
		}

		[TestMethod("Create environment with invalid name.")]
		public void CreateEnvironmentWithInvalidName_ReturnsBadRequest()
		{
			Environment2D environment = new()
			{
				ID = Guid.NewGuid(),
				UserID = Guid.Empty, // UserID wordt hier niet gebruikt. 
				Name = "This is my new testing world with way too many characters for the name.",
				Width = 1000,
				Height = 1000
			};

			ActionResult<Guid> actionResult = _environmentController.CreateEnvironment(environment);

			Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult.Result);

			_environments.Clear();
		}

		[TestMethod("Create environment with maximum capacity.")]
		public void CreateEnvironmentWithMaximumCapacity_ReturnsBadRequest()
		{
			for (int i = 0; i < 10; i++) {
				Environment2D environment = new()
				{
					ID = Guid.NewGuid(),
					UserID = Guid.Empty, // UserID wordt hier niet gebruikt. 
					Name = $"Testing World {i}",
					Width = 50,
					Height = 50
				};

				ActionResult<Guid> actionResult = _environmentController.CreateEnvironment(environment);

				if (i < 5) {
					Assert.IsInstanceOfType<OkObjectResult>(actionResult.Result);
				} else {
					Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult.Result);
				}
			}

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
		public void UpdateEnvironmentWithInvalidId_ReturnsNotFound()
		{
			Guid newEnvironmentId = Guid.NewGuid();
			string updatedName = "Testing World 1";

			Environment2D newEnvironment = new()
			{
				ID = newEnvironmentId,
				UserID = _currentUserId,
				Name = "Testing World 0",
				Width = 10,
				Height = 50
			};

			_environments.Add(newEnvironment);

			Environment2D updatedEnvironment = new()
			{
				ID = Guid.NewGuid(),
				UserID = _currentUserId,
				Name = updatedName,
				Width = 100,
				Height = 75
			};

			ActionResult actionResult = _environmentController.UpdateEnvironment(updatedEnvironment);

			Assert.IsInstanceOfType<NotFoundObjectResult>(actionResult);
			Assert.IsFalse(_environments.Where(environment => environment.Name == updatedName).Any());

			_environments.Clear();
		}

		[TestMethod("Update environment with invalid width and height.")]
		public void UpdateEnvironmentWithInvalidSize_ReturnsBadRequest()
		{
			Guid newEnvironmentId = Guid.NewGuid();
			string updatedName = "Testing World 1";

			Environment2D newEnvironment = new()
			{
				ID = newEnvironmentId,
				UserID = _currentUserId,
				Name = "Testing World 0",
				Width = 10,
				Height = 50
			};

			_environments.Add(newEnvironment);

			Environment2D updatedEnvironment = new()
			{
				ID = newEnvironmentId,
				UserID = _currentUserId,
				Name = updatedName,
				Width = 1000,
				Height = 750
			};

			ActionResult actionResult = _environmentController.UpdateEnvironment(updatedEnvironment);

			Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult);
			Assert.IsFalse(_environments.Where(environment => environment.Name == updatedName).Any());

			_environments.Clear();
		}

		[TestMethod("Update environment with valid environment id.")]
		public void UpdateEnvironmentWithValidId_ReturnsOk()
		{
			Guid newEnvironmentId = Guid.NewGuid();
			string updatedName = "Testing World 1";

			Environment2D newEnvironment = new()
			{
				ID = newEnvironmentId,
				UserID = _currentUserId,
				Name = "Testing World 0",
				Width = 10,
				Height = 50
			};

			_environments.Add(newEnvironment);

			Environment2D updatedEnvironment = new()
			{
				ID = newEnvironmentId,
				UserID = _currentUserId,
				Name = updatedName,
				Width = 20,
				Height = 70
			};

			ActionResult actionResult = _environmentController.UpdateEnvironment(updatedEnvironment);

			Assert.IsInstanceOfType<OkResult>(actionResult);
			Assert.IsTrue(_environments.Where(environment => environment.Name == updatedName).Any());

			_environments.Clear();
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
