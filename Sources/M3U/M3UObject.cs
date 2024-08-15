using System.Text;

namespace iptv_manager_maui.Sources.M3U
{
	public class M3UObject
	{
		public string GroupTitle = "";
		public string TvgName = "";
		public string TvgLogo = "";
		public string TvgId = "";
		public string UrlTvg = "";
		public DateTime Date = DateTime.MinValue;

		private bool? possibleLiveStream = null;
		public (string,int,int)? tvShowInfo = null;

		public bool isTvShow
		{
			get
			{
				if (tvShowInfo == null)
				{
					return checkifTvShow();
				}
				var (a, b, c) = tvShowInfo.Value;
				return b > 0;
			}
		}

		private bool checkifTvShow()
		{
			var name = TvgName;
			int len = name.Length;
			int s = -1;
			int e = -1;
			int remIndex = -1;
			for (int i = 0; i < len; i++)
			{
				char ch = name[i];
				if (s == -1 && ch is 'S' or 's')
				{
					try
					{
						char s0 = i+1 < len ? name[i+1] : '\0';
						char s1 = i+2 < len ? name[i+2] : '\0';
						if (char.IsDigit(s0) && char.IsDigit(s1))
						{
							s = (s0 - '0') * 10 + (s1 - '0');
						}
						else if (char.IsDigit(s0))
						{
							s = (s0 - '0');
						}
						if (s != -1)
						{
							for (int j = i - 1; j >= 0; j--)
							{
								if (char.IsWhiteSpace(name[j]) == false)
								{
									remIndex = j + 1;
									break;
								}
							}
						}
					}
					catch
					{ }
				}

				if (s != -1 && ch is 'E' or 'e')
				{
					try
					{
						char e0 = i+1 < len ? name[i+1] : '\0';
						char e1 = i+2 < len ? name[i+2] : '\0';
						if (char.IsDigit(e0) && char.IsDigit(e1))
						{
							e = (e0 - '0') * 10 + (e1 - '0');
						}
						else if (char.IsDigit(e0))
						{
							e = (e0 - '0');
						}
					}
					catch { }
				}
			}
			if (s == -1 || e == -1)
			{
				tvShowInfo = ("", 0, 0);
				return false;
			}
			tvShowInfo = (remIndex > 0 ? name.Remove(remIndex) : name, s, e);
			return true;
		}

		public bool PossibleLiveStream
		{
			get
			{
				if (possibleLiveStream == null)
				{
					int i = UrlTvg.LastIndexOf('/');
					possibleLiveStream = (i >= 0 ? UrlTvg.IndexOf('.', i) == -1 : false);
				}
				return possibleLiveStream == true;
			}
		}

		public string getID(StringBuilder? sb = null)
		{
			if (string.IsNullOrEmpty(TvgId) == false)
				return TvgId;
			if (sb == null)
			{
				return TvgId = $"{GroupTitle}{WatchableObject.Tok}{TvgName}";
			}
			sb.Clear();
			sb.Append(GroupTitle);
			sb.Append(WatchableObject.Tok);
			sb.Append(TvgName);
			return TvgId = sb.ToString();
		}

		public override string ToString()
		{
			return TvgName;
		}
	}

	public class WatchableObject
	{
		public static string Tok = " - ";
	}
}
