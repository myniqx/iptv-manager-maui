using iptv_manager_maui.Sources.Database;
using iptv_manager_maui.Sources.Database.Models;

namespace iptv_manager_maui.Sources.M3U
{
	public class M3ULoader
	{
		private M3UDatabase _context { get; }
		private Profile profile { get; }

		public M3ULoader(M3UDatabase context, int profileID)
		{
			_context = context;
			Profile? _profile = _context.Profiles.Where(p => p.ID == profileID).FirstOrDefault();
			if (_profile == null)
				throw new Exception("Profile not found");

			profile = _profile;
		}

		public async void Load()
		{
			var fi = new FileInfo(profile.GetM3UFilePath);
			var parser = new M3UParser(fi);
			parser.ProgressChanged += (sender, e) =>
			{

			};
			List<M3UObject>? objects = await parser.LoadM3U().ConfigureAwait(false);

			AddM3UList(objects);

		}

		private void AddM3UList(List<M3UObject> objects)
		{
			var baseGroup = profile.BaseGroup;
			if (baseGroup.ChildGroups.Count == 0)
			{
				AddWithFilter(baseGroup, objects);
			}
			else
			{
				AddWithoutFilter(baseGroup, objects);
			}
		}

		private void AddWithoutFilter(Group group, List<M3UObject> objects)
		{
			foreach (var obj in objects)
			{
				Add(obj);
			}
		}

		private M3UEntitiy Add(M3UObject obj)
		{
			var group = AddGroup(obj.GroupTitle);
			return AddM3U(group, obj);
		}

		private Group AddGroup(string title)
		{
			var group = profile.BaseGroup.ChildGroups.Where(g => g.Title == title).FirstOrDefault();

			if (group == null)
			{
				group = new Group
				{
					Title = title
				};
				profile.BaseGroup.ChildGroups.Add(group);
				_context.SaveChanges();
			}

			return group;
		}

		private M3UEntitiy AddM3U(Group group, M3UObject obj)
		{
			if (obj.isTvShow)
			{
				var (tvShowGroup, Season, Episode) = obj.tvShowInfo!.Value;
				var tvShowGroup = AddTvGroup(group, tvShowGroup);
				var tvShowSeason = AddTvSeason(tvShowGroup, Season);
			}
		}

	}
}
