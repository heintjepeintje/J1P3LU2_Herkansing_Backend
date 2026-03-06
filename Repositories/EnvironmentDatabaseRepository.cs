using Dapper;
using LU2_API_Herkansing.Interfaces;
using LU2_API_Herkansing.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.Common;

namespace LU2_API_Herkansing.Repositories
{
	public class EnvironmentDatabaseRepository(string sqlConnectionString) : IEnvironmentRepository
	{
		private readonly string _sqlConnectionString = sqlConnectionString;

		public void CreateEnvironment(Environment2D environment)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute(
				"INSERT INTO [Environments] (ID, UserID, Name, Width, Height) VALUES (@ID, @UserID, @Name, @Width, @Height)",
				environment);
		}

		public Environment2D? GetEnvironmentById(Guid environmentId)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			Environment2D? environment = sqlConnection.QuerySingleOrDefault<Environment2D>(
				"SELECT * FROM [Environments] WHERE ID = @ID",
				new { ID = environmentId });

			return environment;
		}

		public IEnumerable<Environment2D> GetEnvironmentsByUser(Guid userId) {
			SqlConnection sqlConnection = new(_sqlConnectionString);

			IEnumerable<Environment2D> environments = sqlConnection.Query<Environment2D>(
				"SELECT * FROM [Environments] WHERE UserID = @UserID",
				new { UserID = userId });

			return environments;
		}


		public void UpdateEnvironment(Environment2D environment)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute(
				"UPDATE [Environments] SET " +
				"Name = @Name" +
				"Width = @Width" +
				"Height = @Height",
				environment);
		}

		public void DeleteEnvironment(Guid environmentId)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute(
				"DELETE FROM [Environments] WHERE ID = @ID",
				new { ID = environmentId });
		}
	}
}
