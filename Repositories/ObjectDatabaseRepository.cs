using Dapper;
using LU2_API_Herkansing.Interfaces;
using LU2_API_Herkansing.Models;
using Microsoft.Data.SqlClient;

namespace LU2_API_Herkansing.Repositories
{
	public class ObjectDatabaseRepository(string sqlConnectionString) : IObjectRepository
	{
		private readonly string _sqlConnectionString = sqlConnectionString;

		public void CreateObject(Object2D obj)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute(
				"INSERT INTO [Objects]" +
				"(ID, EnvironmentID, PrefabID, X, Y, Width, Height, Rotation, Layer) VALUES" +
				"(@ID, @EnvironmentID, @PrefabID, @X, @Y, @Width, @Height, @Rotation, @Layer)",
				obj);
		}
		
		public Object2D? GetObject(Guid id)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			return sqlConnection.QuerySingleOrDefault<Object2D>(
				"SELECT * FROM [Objects] WHERE ID = @ID",
				new { id });
		}

		public IEnumerable<Object2D> GetEnvironmentObjects(Guid environmentId)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			return sqlConnection.Query<Object2D>(
				"SELECT * FROM [Objects] WHERE EnvironmentID = @EnvironmentID",
				new { environmentId });
		}

		public void UpdateObject(Object2D obj)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute(
				"UPDATE [Objects] SET " +
				"PrefabID = @PrefabID, X = @X, Y = @Y, Width = @Width, Height = @Height, Rotation = @Rotation, Layer = @Layer " +
				"WHERE ID = @ID AND EnvironmentID = @EnvironmentID",
				obj);
		}

		public void DeleteObject(Guid id)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute("DELETE FROM [Objects] WHERE ID = @ID", new { ID = id });
		}

		public void DeleteEnvironmentObjects(Guid environmentId) {
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute("DELETE FROM [Objects] WHERE EnvironmentID = @EnvironmentID", new { EnvironmentID = environmentId });
		}
	}
}
