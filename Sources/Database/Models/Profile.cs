using iptv_manager_maui.Sources.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iptv_manager_maui.Sources.Database.Models
{
	[Index(nameof(Name), IsUnique = true)]
	public class Profile
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

		[Required]
		public string Url { get; set; } = "";

		[Required]
		public string Name { get; set; } = "";

		[Required]
		public string M3UFile { get; set; } = $"{Guid.NewGuid()}.m3u";

		public DateTime CreatedDate { get; set; } = DateTime.Now;

		public DateTime? LastUpdateDate { get; set; }

		public int GroupCount { get; set; } = 0;

		public int TotalCount { get; set; } = 0;

		public int LiveStreamCount { get; set; } = 0;

		public int MovieCount { get; set; } = 0;

		public int TvShowCount { get; set; } = 0;

		public int TvShowSeasonCount { get; set; } = 0;

		public int TvShowEpisodeCount { get; set; } = 0;

		public int BaseGroupID { get; set; }

		[ForeignKey("BaseGroupID")]
		public Group BaseGroup { get; set; } = null!;

		[NotMapped]
		public string GetM3UFilePath => IpTvConfig.GetM3UFilePath(M3UFile);

	}
}
