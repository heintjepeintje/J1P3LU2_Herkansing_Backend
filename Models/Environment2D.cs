using System.ComponentModel.DataAnnotations;

namespace LU2_API_Herkansing.Models
{
	public class Environment2D
	{
		
		public Guid ID { get; set; }

		public Guid UserID { get; set; }

		[Required]
		public string? Name { get; set; }

		[Required]
		public int Width { get; set; }

		[Required]
		public int Height { get; set; }
	}
}
