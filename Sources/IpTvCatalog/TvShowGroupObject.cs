using System;
using System.Linq;

namespace IpTvParser.IpTvCatalog
{
	public class TvShowGroupObject : GroupObject
	{
		public override object GetIcon => MainWindow.TvShowIcon;
		public override string GetTitle => "Tv Show";

		public override int SeasonCount => Members.Count;

		public override int MovieCount => 0;

		public override int EpisodeCount
		{
			get
			{
				int total = 0;
				foreach (var member in Members)
				{
					if (member is TvShowSeasonGroupObject t)
						total += t.EpisodeCount;
				}
				return total;
			}
		}

		public TvShowSeasonGroupObject AddSeason(int season)
		{
			season = Math.Max(1, season);
			var name = $"Season {season}";
			var group = Members.FirstOrDefault(x => x is TvShowSeasonGroupObject && string.Compare(x.Name,name) == 0);
			if (group != null)
				return (TvShowSeasonGroupObject)group;
			var groupObject = new TvShowSeasonGroupObject{ Season = season , Name = name, UpperLevel = this};
			Members.Add(groupObject);
			return groupObject;
		}

		public override string ToString()
		{
			return $"{Name} : {Members.Count(m => m is TvShowSeasonGroupObject)} Season(s) and {TvShowCount} Episode(s).";
		}
	}

	public class TvShowSeasonGroupObject : GroupObject
	{
		public int Season { get; set; } = 1;
		public override object GetIcon => MainWindow.TvShowIcon;
		public override string GetTitle => $"Season {Season}";

		public override int EpisodeCount => Members.Count;

		public override int MovieCount => 0;

		public override string ToString()
		{
			return $"{Season}. Season with {Members.Count} Episodes.";
		}
	}
}
