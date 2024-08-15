using IpTvParser.Helpers;
using System.Text;

namespace IpTvParser.IpTvCatalog
{
	public class WatchableObject : ViewObject
	{
		public string Url = "";

		public string Group = "";

		public enum EList { NONE, WATCH, WATCHED };

		public M3UObject GetM3UObject => new()
		{
			GroupTitle = Group,
			TvgLogo = Logo,
			TvgName = Name,
			UrlTvg = Url,
			Date = AddedDate
		};

		public override object GetListIcon
		{
			get => _elist switch
			{
				EList.WATCH => MainWindow.WatchIcon,
				EList.WATCHED => MainWindow.WatchedIcon,
				_ => MainWindow.NullIcon
			};
		}

		EList _elist = EList.NONE;
		public EList eList
		{
			get => _elist;
			set
			{
				_elist = value;
				propertyChanged
					(nameof(GetListIcon));
			}
		}

		public override object GetIcon => PossibleLiveStream ? MainWindow.LiveStreamIcon : MainWindow.FilmIcon;
		public override string GetTitle => PossibleLiveStream ? "LiveStream" : "Movie";

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

		public static string Tok = "$@$";

		public string ID { get; set; } = "";
		public override string ToString()
		{
			StringBuilder sb = new();
			sb.Append("Group : ").Append(Group).AppendLine();
			sb.Append("Name  : ").Append(Name).AppendLine();
			sb.Append("Url   : ").Append(Url).AppendLine();
			sb.Append("Logo  : ").Append(Logo).AppendLine();
			return sb.ToString();
		}
	}

}
