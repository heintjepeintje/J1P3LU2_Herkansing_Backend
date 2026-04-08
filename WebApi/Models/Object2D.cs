using System.ComponentModel.DataAnnotations;

namespace LU2_API_Herkansing.Models
{
	public class Object2D
	{
		public Guid ID { get; set; }

		[Required]
		public Guid EnvironmentID { get; set; }

		[Required]
		public string? PrefabID { get; set; }

		[Required]
		public int X { get; set; }

		[Required]
		public int Y { get; set; }

		[Required]
		public int Width { get; set; }

		[Required]
		public int Height { get; set; }

		[Required]
		public int Rotation { get; set; }

		[Required]
		public int Layer { get; set; }
	}
}
