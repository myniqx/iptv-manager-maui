namespace iptv_manager_maui.Sources.Catalog
{
	internal class GroupObject : BaseObject
	{
		public GroupObject(string name, GroupObject upperLevel)
		: base(name, upperLevel)
		{
		}

		public List<WatchableObject> Watchables { get; set; } = new();

		public List<GroupObject> Groups { get; set; } = new();

		public GroupObject AddGroup(string groupName)
		{
			if (string.IsNullOrEmpty(groupName))
				groupName = "unnamed group";

			var group = Groups.FirstOrDefault(x => x.Name == groupName);

			if (group != null)
				return group;

			group = new GroupObject(groupName, this);
			Groups.Add(group);
			return group;
		}
	}
}
