using LU2_API_Herkansing.Models;

namespace LU2_API_Herkansing.Interfaces
{
	public interface IUserEnvironmentRepository
	{
		public void CreateUserEnvironment(UserEnvironment userEnvironment);
		public Guid GetEnvironmentOwner(Guid environmentId);
		public IEnumerable<UserEnvironment> GetUserEnvironmentsByUserId(Guid userId);
		public IEnumerable<UserEnvironment> GetUserEnvironmentsByEnvironmentId(Guid environmentId);
		public void DeleteUserEnvironment(UserEnvironment userEnvironment);
	}
}
