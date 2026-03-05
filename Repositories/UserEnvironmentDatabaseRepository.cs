using Dapper;
using LU2_API_Herkansing.Interfaces;
using LU2_API_Herkansing.Models;
using Microsoft.Data.SqlClient;


namespace LU2_API_Herkansing.Repositories
{
	public class UserEnvironmentDatabaseRepository(string sqlConnectionString) : IUserEnvironmentRepository
	{
		private readonly string _sqlConnectionString = sqlConnectionString;

		public void CreateUserEnvironment(UserEnvironment userEnvironment)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute(
				"INSERT INTO [UserEnvironments] (UserID, EnvironmentID) VALUES (@UserID, @EnvironmentID)",
				userEnvironment);
		}

		public Guid? GetEnvironmentOwner(Guid environmentId)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			Guid? userId = sqlConnection.QuerySingleOrDefault<Guid>(
				"SELECT ID FROM [UserEnvironments] WHERE EnvironmentID = @EnvironmentID AND Owned = 1",
				new { EnvironmentID = environmentId });

			return userId;
		}

		public IEnumerable<UserEnvironment> GetUserEnvironmentsByUserId(Guid userId)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			IEnumerable<UserEnvironment> environments = sqlConnection.Query<UserEnvironment>(
				"SELECT * FROM [UserEnvironments] WHERE UserID = @UserID",
				new { userId });

			return environments;
		}

		public IEnumerable<UserEnvironment> GetUserEnvironmentsByEnvironmentId(Guid environmentId)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			IEnumerable<UserEnvironment> users = sqlConnection.Query<UserEnvironment>(
				"SELECT * FROM [UserEnvironments] WHERE EnvironmentID = @EnvironmentID",
				new { environmentId });

			return users;
		}

		public void DeleteUserEnvironment(UserEnvironment userEnvironment)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute(
				"DELETE FROM [UserEnvironments] WHERE UserID = @UserID AND EnvironmentID = @EnvironmentID",
				userEnvironment);
		}
	}
}
