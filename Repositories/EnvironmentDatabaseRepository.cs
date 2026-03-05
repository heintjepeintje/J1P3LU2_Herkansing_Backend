using System;
using Dapper;
using System.Data.Common;
using LU2_API_Herkansing.Models;
using Microsoft.Data.SqlClient;
using LU2_API_Herkansing.Interfaces;

namespace LU2_API_Herkansing.Repositories
{
	public class EnvironmentDatabaseRepository(string sqlConnectionString) : IEnvironmentRespository
	{
		private readonly string _sqlConnectionString = sqlConnectionString;

		public void CreateEnvironment(Environment2D environment)
		{
			SqlConnection sqlConnection = new(_sqlConnectionString);

			sqlConnection.Execute(
				"INSERT INTO [Environments] (ID, Name, Width, Height) VALUES (@ID, @Name, @Width, @Height)",
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
				new { environmentId });
		}
	}
}
