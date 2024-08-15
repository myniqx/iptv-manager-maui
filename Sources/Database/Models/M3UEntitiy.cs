using iptv_manager_maui.Sources.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iptv_manager_maui.Sources.Database.Models
{
	public class M3UEntitiy
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

		[Required]
		public int ProfileID { get; set; }

		[ForeignKey("ProfileID")]
		public virtual Profile Profile { get; set; } = null!;

		[Required]
		public string Name { get; set; } = "";

		public string Logo { get; set; } = "";

		[Required]
		public int GroupID { get; set; }

		[ForeignKey("GroupID")]
		public Group? UpperGroup { get; set; } = null;

		[Required]
		public string Url { get; set; } = "";

		public DateTime LastUpdate { get; set; } = DateTime.Now;

		public EStatus Status { get; set; } = EStatus.None;

		public EListType ListType { get; set; } = EListType.None;

		public int Year { get; set; } = 0;
	}
}
