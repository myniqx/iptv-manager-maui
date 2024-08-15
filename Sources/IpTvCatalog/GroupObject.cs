using IpTvParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace IpTvParser.IpTvCatalog
{
	public class GroupObject : ViewObject
	{
		public GroupObject()
		{
			Members = new();
			Name = "Group";
		}

		public IEnumerable<WatchableObject> Iterator => ForEach(this);

		private static IEnumerable<WatchableObject> ForEach(GroupObject group)
		{
			foreach (var item in group.Members)
			{
				if (item is GroupObject g)
				{
					foreach (var jitem in ForEach(g))
						yield return jitem;
				}
				else if (item is WatchableObject w)
					yield return w;
			}
		}
		public List<ViewObject> Members { get; set; }

		public override DateTime AddedDate => Members.Max(m => m.AddedDate);

		public override string GetTitle { get => "Group"; }

		public override string Logo
		{
			get
			{
				return Members?.FirstOrDefault()?.Logo ?? "";
			}
		}

		protected virtual bool shouldFilter(ViewObject v)
		{
			return UpperLevel?.shouldFilter(v) ?? false;
		}

		public IEnumerable<ViewObject> GMembers
		{
			get => Members.Where(m => shouldFilter(m));
		}

		public void notifyGMember()
		{
			propertyChanged(nameof(GMembers));
		}

		public int GetTotal
		{
			get
			{
				int total = 0;
				foreach (var m in Members)
				{
					if (m is GroupObject g)
						total += g.GetTotal;
					else if (m is WatchableObject)
						total++;
				}
				return total;
			}
		}

		public int LiveStreamCount
		{
			get
			{
				int total = 0;
				foreach (var m in Members)
				{
					if (m is TvShowGroupObject)
						continue;
					if (m is GroupObject g)
						total += g.LiveStreamCount;
					else if (m is WatchableObject w && w.PossibleLiveStream)
						total++;
				}
				return total;
			}
		}

		public int TvShowSeasonCount
		{
			get
			{
				int total = 0;
				foreach (var m in Members)
				{
					if (m is TvShowGroupObject tvshow)
						total += tvshow.SeasonCount;
					else if (m is GroupObject group)
						total += group.TvShowSeasonCount;
				}
				return total;
			}
		}

		public int TvShowEpisodeCount
		{
			get
			{
				int total = 0;
				foreach (var m in Members)
				{
					if (m is TvShowGroupObject tvshow)
						total += tvshow.EpisodeCount;
					else if (m is GroupObject group)
						total += group.TvShowEpisodeCount;
				}
				return total;
			}
		}

		public bool AnyRecently => Members.Any(m => m is WatchableObject w && w.AddedDate != null || m is GroupObject g && g.AnyRecently);
		public Visibility RecentlyVisible => AnyRecently ? Visibility.Visible : Visibility.Collapsed;

		public virtual int SeasonCount => 0;

		public virtual int EpisodeCount => 0;

		public virtual int MovieCount
		{
			get
			{
				int total = 0;
				foreach (var m in Members)
				{
					if (m is TvShowGroupObject or TvShowSeasonGroupObject)
						continue;
					if (m is GroupObject g)
						total += g.MovieCount;
					else if (m is WatchableObject w && w.PossibleLiveStream == false)
						total++;
				}
				return total;
			}
		}

		public int TvShowCount
		{
			get
			{
				int total = 0;
				foreach (var m in Members)
				{
					if (m is TvShowGroupObject g)
						total++;
					else if (m is GroupObject go)
						total += go.TvShowCount;
				}
				return total;
			}
		}

		public virtual void lastCheck()
		{
			for (int i = Members.Count - 1; i >= 0; i--)
			{
				if (Members[i] is GroupObject gr)
				{
					gr.lastCheck(); //first get tidy
					if (gr.Count == 0) //then if its empty, remove it
						Members.RemoveAt(i);
				}
			}
			Members.Sort();
		}

		public int Count
		{
			get { return Members.Count; }
		}

		public virtual WatchableObject Add(M3UObject m3u)
		{
			var tvShow = TvShowWatchableObject.CreateIfTvShow(m3u.TvgName);

			if (tvShow != null)
			{
				return Add(m3u, tvShow);
			}

			var obj = new WatchableObject();

			obj.Name = m3u.TvgName;
			obj.Url = m3u.UrlTvg;
			obj.Logo = m3u.TvgLogo;
			obj.Group = m3u.GroupTitle;
			obj.ID = m3u.TvgId;
			obj.AddedDate = m3u.Date;
			obj.UpperLevel = this;
			Members.Add(obj);
			return obj;
		}

		public void justAdd(WatchableObject obj)
		{
			Members.Add(obj);
			propertyChanged(nameof(Count));
		}

		public void justRemove(WatchableObject obj)
		{
			Members.Remove(obj);
			propertyChanged(nameof(Count));
		}

		protected virtual WatchableObject Add(M3UObject m3u, TvShowWatchableObject tvShow)
		{
			var tvShowGroup = AddTvGroup(tvShow.Group);
			var tvShowSeazon = tvShowGroup.AddSeason(tvShow.Season);

			if (m3u.TvgId != null)
				tvShow.ID = m3u.TvgId;
			tvShow.Name = m3u.TvgName;
			tvShow.Url = m3u.UrlTvg;
			tvShow.Logo = m3u.TvgLogo;
			tvShow.AddedDate = m3u.Date;
			tvShow.UpperLevel = tvShowSeazon;
			tvShowSeazon.Members.Add(tvShow);
			return tvShow;
		}

		public WatchableObject? findObject(string name)
		{
			foreach (var m in Members)
			{
				if (m is GroupObject g)
				{
					var o = g.findObject(name);
					if (o != null)
						return o;
				}
				else if (m is WatchableObject w)
				{
					if (string.Compare(w.Name, name) == 0)
						return w;
				}
			}
			return null;
		}

		public GroupObject AddGroup(String groupName)
		{
			if (string.IsNullOrEmpty(groupName))
				groupName = "unnamed group";
			var group = Members.FirstOrDefault(x => x is GroupObject && string.Compare(x.Name,groupName) == 0);
			if (group != null)
				return (GroupObject)group;
			var groupObject = new GroupObject{ Name = groupName,UpperLevel = this};
			Members.Add(groupObject);
			return groupObject;
		}

		public TvShowGroupObject AddTvGroup(String groupName)
		{
			if (string.IsNullOrEmpty(groupName))
				groupName = "unnamed tvshow";
			var group = Members.FirstOrDefault(x => x is TvShowGroupObject && string.Compare(x.Name,groupName) == 0);
			if (group != null)
				return (TvShowGroupObject)group;
			var groupObject = new TvShowGroupObject{ Name = groupName, UpperLevel = this };
			Members.Add(groupObject);
			return groupObject;
		}

		public void SearchThrough(string[] parameters, List<ViewObject> list, CancellationTokenSource source)
		{
			foreach (var m in Members)
			{
				if (source.IsCancellationRequested)
					return;
				if (m is TvShowGroupObject tv)
				{
					if (tv.hasMatch(parameters))
						list.Add(tv);
				}
				else if (m is GroupObject go)
				{
					go.SearchThrough(parameters, list, source);
				}
				else if (m is WatchableObject wo)
				{
					if (wo.hasMatch(parameters))
						list.Add(wo);
				}
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new(1024);
			sb.Append("Group : ").AppendLine(Name);
			int livestream = LiveStreamCount;
			if (livestream > 0)
				sb.Append("LiveStream : ").Append(livestream).AppendLine();
			int tvshow = TvShowCount;
			if (tvshow > 0)
				sb.Append("Tv Show : ").Append(tvshow).AppendLine();
			int movie = MovieCount;
			if (tvshow > 0)
				sb.Append("Movie : ").Append(movie).AppendLine();
			sb.Append("Total : ").Append(Count).AppendLine();
			return sb.ToString();
		}


	}

}
