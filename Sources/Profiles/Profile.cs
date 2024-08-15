using Downloader;
using iptv_manager_maui.Sources.M3U;
using iptv_manager_maui.Sources.Profiles;
using iptv_manager_maui.Sources.Utils;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace iptv_manager_maui.Sources.Settings
{
	public class Profile
	{
		public string Url { get; set; } = "";
		public string Name { get; set; } = "";
		public string M3UFile { get; set; } = "";
		public int ID { get; set; } = 0;
		public string lastUpdate
		{
			get
			{
				if (LastUpdateDate == null)
					return "";
				var date = (DateTime)LastUpdateDate;
				if (date.CompareTo(CreatedDate) == 0)
					return "just added";
				var span = DateTime.Now.Subtract(date);
				return $"{span.Days} day(s) ago";
			}
		}

		public DateTime CreatedDate { get; set; }
		public DateTime? LastUpdateDate { get; set; } = null;

		[JsonIgnore]
		private static ProfileManager profileManager = ProfileManager.Instance;

		public int GroupCount
		{
			get => _groupCount;
			set
			{
				if (value != _groupCount)
					profileManager.ResetHeartBeat();
				_groupCount = value;
			}
		}

		public int TotalCount
		{
			get => _totalCount;
			set
			{
				if (value != _totalCount)
					profileManager.ResetHeartBeat();
				_totalCount = value;
			}
		}

		int _groupCount = 0;
		int _totalCount = 0;
		int _liveStreamCount = 0;
		int _movieCount = 0;
		int _tvShowCount = 0;
		int _tvShowSeasonCount = 0;
		int _tvShowEpisodeCount = 0;

		public int TvShowSeasonCount
		{
			set
			{
				_tvShowSeasonCount = value;
				profileManager.ResetHeartBeat();
			}
			get => _tvShowSeasonCount;
		}

		public int TvShowEpisodeCount
		{
			set
			{
				_tvShowEpisodeCount = value;
				profileManager.ResetHeartBeat();
			}
			get => _tvShowEpisodeCount;
		}

		public int LiveStreamCount
		{
			set
			{
				_liveStreamCount = value;
				profileManager.ResetHeartBeat();
			}
			get => _liveStreamCount;
		}

		public int MovieCount
		{
			set
			{
				_movieCount = value;
				profileManager.ResetHeartBeat();
			}
			get => _movieCount;
		}

		public int TvShowCount
		{
			set
			{
				_tvShowCount = value;
				profileManager.ResetHeartBeat();
			}
			get => _tvShowCount;
		}

		[JsonIgnore]
		public string ProfileSource
		{
			get
			{
				if (isLocal)
					return $"local file: {M3UFile}";
				return Url;
			}
		}
		public bool isLocal { get; set; }

		[JsonIgnore]
		public Visibility isUpdatable
		{
			get => isLocal ? Visibility.Hidden : Visibility.Visible;
		}

		public Dictionary<string, EList> listedItems { get; set; } = new();
		public Dictionary<DateTime, List<string>> latelyAdded { get; set; } = new();
		public HashSet<string> bannedGroups { get; set; } = new();
		public HashSet<string> stickyGroups { set; get; } = new();

		[JsonIgnore]
		public string GetM3UFilePath => IpTvConfig.GetM3UFilePath(M3UFile);

		private static string tempName(int id) => $"profile#{id}";
		public Profile(string url, int id)
		{
			this.Url = url;
			ID = id;
			Name = tempName(id);
			CreatedDate = DateTime.Now;
			LastUpdateDate = null;
			M3UFile = string.Format("{0}.m3u", CreatedDate.Ticks);
			isLocal = false;
			catalog = new Catalog(this);
		}

		public Profile(FileInfo fi, int id)
		{
			this.Url = "";
			ID = id;
			Name = tempName(id);
			CreatedDate = DateTime.Now;
			LastUpdateDate = null;
			M3UFile = fi.Name;
			File.Copy(fi.FullName, GetM3UFilePath, true);
			isLocal = true;
			catalog = new Catalog(this);
		}

		public Profile()
		{
			Url = "";
			Name = "";
			M3UFile = "";
			isLocal = true;
			catalog = new Catalog(this);
		}

		[JsonIgnore]
		string _status = "Ready";

		[JsonIgnore]
		public string Status
		{
			get => _status;
			set
			{
				_status = value;
				profileManager.ResetHeartBeat();
			}
		}

		public void WriteLine(string str)
		{
			Console.WriteLine($"[{Name}] > {str}");
		}

		public async Task<bool> loadProfile(bool forceDownload = false)
		{
			if (isLocal && isLoaded)
			{
				Status = "profile already loaded.";
				return true;
			}

			Status = "profile begins to load.";

			if (!isLoaded)
			{
				await LoadM3UFromFileAsync().ConfigureAwait(false);
			}

			if (forceDownload || !File.Exists(GetM3UFilePath))
			{
				if (await DownloadM3UFileAsync().ConfigureAwait(false))
				{
					await LoadM3UFromFileAsync().ConfigureAwait(false);
				}
				else
					return false;
			}

			if (isLocal || isLoaded)
			{
				Status = "profile loaded.";
				return true;
			}

			return false;
		}

		private async Task<bool> LoadM3UFromFileAsync()
		{
			var fi = new FileInfo(GetM3UFilePath);
			if (fi.Exists == false)
			{
				return isLoaded = false;
			}
			var parser = new M3UParser(fi);
			parser.ProgressChanged += (o, p) =>
			{
				Status = $"m3u's parsing : %{p:f2}";
			};
			var m3ulist = await parser.LoadM3U().ConfigureAwait(false);
			if (m3ulist.Count == 0)
			{
				return isLoaded = false;
			}
			Status = "parse done, getting ready...";
			catalog.AddM3UList(m3ulist);

			GroupCount = catalog.movieList.Count;
			TvShowCount = catalog.movieList.TvShowCount;
			TvShowSeasonCount = catalog.movieList.TvShowSeasonCount;
			TvShowEpisodeCount = catalog.movieList.TvShowEpisodeCount;
			LiveStreamCount = catalog.movieList.LiveStreamCount;
			MovieCount = catalog.movieList.MovieCount;
			TotalCount = TvShowEpisodeCount + MovieCount + LiveStreamCount;

			return isLoaded = true;
		}

		private async Task<bool> DownloadM3UFileAsync(bool loadAfterDownload = false)
		{
			DownloadService service = new();
			bool successful = false;
			service.DownloadStarted += (_e, _o) =>
			{
				Status = "m3u file is downloading...";
				WriteLine("m3u file is downloading...");
				WriteLine("url is " + Url);
			};
			service.DownloadProgressChanged += (_e, _o) =>
			{
				Status =
				$"Downloading: Speed: {_o.BytesPerSecondSpeed.toSize(),-8}, {_o.ReceivedBytesSize.toSize(),-8} / {_o.TotalBytesToReceive.toSize(),-8} %{_o.ProgressPercentage,-5:f1}";
			};
			service.DownloadFileCompleted += (_e, _o) =>
			{
				successful = service.Package.IsSaveComplete;
				if (successful == false)
				{
					WriteLine($"download stopped with error : {_o.Error?.Message ?? ""}");
					Status = "Download stopped with error.";
				}
				else
				{
					WriteLine("download compeleted.");
					Status = "Downlod completed.";
					LastUpdateDate = DateTime.Now;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lastUpdate)));
					isLoaded = false;
				}
			};
			await service.DownloadFileTaskAsync(Url, GetM3UFilePath).ConfigureAwait(false);
			return successful;
		}

		[JsonIgnore]
		private bool isLoaded { get; set; }

		public void WacthList(WatchableObject obj)
		{
			if (obj.eList == EList.WATCH)
				return;
			obj.eList = EList.WATCH;
			catalog.watchList.justAdd(obj);
			catalog.watchedList.justRemove(obj);
			listedItems[obj.ID] = EList.WATCH;
		}

		public void WacthedList(WatchableObject obj)
		{
			if (obj.eList == EList.WATCHED)
				return;
			obj.eList = EList.WATCHED;
			catalog.watchedList.justAdd(obj);
			catalog.watchList.justRemove(obj);
			listedItems[obj.ID] = EList.WATCHED;
		}

		public void RemoveFromList(WatchableObject obj)
		{
			if (obj.eList == EList.NONE)
				return;
			obj.eList = EList.NONE;
			catalog.watchedList.justRemove(obj);
			catalog.watchList.justRemove(obj);
			listedItems.Remove(obj.ID);
		}

		[JsonIgnore]
		public Catalog catalog { get; set; }
	}

	public enum EList
	{
		NONE,
		WATCH,
		WATCHED
	}
}