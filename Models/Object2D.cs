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
		public float X { get; set; }

		[Required]
		public float Y { get; set; }

		[Required]
		public float Width { get; set; }

		[Required]
		public float Height { get; set; }

		[Required]
		public float Rotation { get; set; }

		[Required]
		public float Layer { get; set; }
	}
}
