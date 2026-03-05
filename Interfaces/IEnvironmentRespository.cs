using LU2_API_Herkansing.Models;

namespace LU2_API_Herkansing.Interfaces
{
	public interface IEnvironmentRespository
	{
		public void CreateEnvironment(Environment2D environment);
		public Environment2D? GetEnvironmentById(Guid id);
		public void UpdateEnvironment(Environment2D environment);
		public void DeleteEnvironment(Guid environment);
	}
}
