using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace LU2_API_Herkansing.Models
{
	public class UserEnvironment
	{
		[Required]
		public Guid UserID { get; set; }

		[Required]
		public Guid EnvironmentID { get; set; }

		[Required]
		public bool Owned { get; set; }
	}
}
