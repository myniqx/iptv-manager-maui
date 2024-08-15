namespace iptv_manager_maui.Sources.Catalog
{
	internal class WatchableObject : BaseObject
	{
		public WatchableObject(string name, string url, string group)
			: base(name)
		{
			Url = url;
			Group = group;
		}

		public string Url { get; set; }
		public string Group { get; set; }
		public ListTypes ListTypes { get; set; } = ListTypes.NONE;

		bool? possibleLiveStream = null;

		public bool PossibleLiveStream
		{
			get
			{
				if (possibleLiveStream == null)
				{
					int i = Url.LastIndexOf('/');
					possibleLiveStream = (i >= 0 ? Url.IndexOf('.', i) == -1 : false);
				}
				return possibleLiveStream == true;
			}
		}

	}
}
