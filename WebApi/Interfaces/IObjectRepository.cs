using LU2_API_Herkansing.Models;

namespace LU2_API_Herkansing.Interfaces
{
	public interface IObjectRepository
	{
		public bool CreateObject(Object2D obj);
		public Object2D? GetObject(Guid id);
		public IEnumerable<Object2D> GetEnvironmentObjects(Guid environmentId);
		public bool UpdateObject(Object2D obj);
		public bool DeleteEnvironmentObjects(Guid environmentId);
		public bool DeleteObject(Guid id);
	}
}
