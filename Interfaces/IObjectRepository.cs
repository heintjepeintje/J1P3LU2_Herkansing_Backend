using LU2_API_Herkansing.Models;

namespace LU2_API_Herkansing.Interfaces
{
	public interface IObjectRepository
	{
		public void CreateObject(Object2D obj);
		public Object2D? GetObject(Guid id);
		public IEnumerable<Object2D> GetEnvironmentObjects(Guid environmentId);
		public void UpdateObject(Object2D obj);
		public void DeleteObject(Guid id);
	}
}
