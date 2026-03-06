using LU2_API_Herkansing.Models;

namespace LU2_API_Herkansing.Interfaces
{
	public interface IEnvironmentRepository
	{
		public void CreateEnvironment(Environment2D environment);
		public Environment2D? GetEnvironmentById(Guid id);
		public IEnumerable<Environment2D> GetEnvironmentsByUser(Guid userId);
		public void UpdateEnvironment(Environment2D environment);
		public void DeleteEnvironment(Guid environment);
	}
}
