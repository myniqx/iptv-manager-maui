namespace iptv_manager_maui.Sources.Catalog
{
	internal class Section : GroupObject
	{
		public Section(string name) : base(name)
		{

		}

		public GroupObject movieList { get; set; }

		public GroupObject tvShowList { get; set; }

		public GroupObject liveStreamList { get; set; }
	}
}
