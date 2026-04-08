using LU2_API_Herkansing.Models;

namespace LU2_API_Herkansing.Interfaces
{
	public interface IEnvironmentRepository
	{
		public bool CreateEnvironment(Environment2D environment);
		public Environment2D? GetEnvironmentById(Guid id);
		public IEnumerable<Environment2D> GetEnvironmentsByUser(Guid userId);
		public bool UpdateEnvironment(Environment2D environment);
		public bool DeleteEnvironment(Guid environment);
	}
}
