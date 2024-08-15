using iptv_manager_maui.Sources.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iptv_manager_maui.Sources.Database.Models
{
	public class Group
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

		[Required]
		public int ProfileID { get; set; }

		[Required]
		public string Title { get; set; } = "";

		[ForeignKey("ProfileID")]
		public virtual Profile Profile { get; set; } = null!;

		public int? ParentGroupID { get; set; }

		[ForeignKey("ParentGroupID")]
		public Group? ParentGroup { get; set; }

		public virtual List<Group> ChildGroups { get; set; } = new();

		public virtual List<M3UEntitiy> M3UObjects { get; set; } = new();

		public EStatus Status { get; set; } = EStatus.None;

		public EListType ListType { get; set; } = EListType.None;

		public DateTime? LastUpdate { get; set; } = null;

		public string Logos { get; set; } = "";

		public EGroupType GroupType { get; set; } = EGroupType.Default;

		public int SeasonNumber { get; set; } = 0;
	}

}
