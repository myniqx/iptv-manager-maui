using iptv_manager_maui.Sources.Settings;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace iptv_manager_maui.Sources.Profiles
{
	internal class ProfileManager
	{
		static string jsonKey = "profiles";

		static ProfileManager? _instance;
		public static ProfileManager Instance
		{
			get
			{
				if (_instance == null)
				{
					try
					{
						var jsonData = Preferences.Get(jsonKey, "{}");
						_instance = JsonSerializer.Deserialize<ProfileManager>(jsonData);
						if (_instance == null)
						{
							_instance = new ProfileManager();
						}
					}
					catch
					{
						_instance = new ProfileManager();
					}
				}
				return _instance;
			}
		}

		public void Save()
		{
			Preferences.Set(jsonKey, JsonSerializer.Serialize(this));
			ResetHeartBeat();
		}

		[JsonIgnore]
		string? _heartBeat = null;
		public string GetHeartBeat()
		{
			if (_heartBeat == null)
			{
				_heartBeat = JsonSerializer.Serialize(Profiles);
			}
			return _heartBeat;
		}

		public void ResetHeartBeat() => _heartBeat = null;

		public ObservableCollection<Profile> Profiles { get; set; } = new();
		public Profile GetProfileFrom(string path)
		{
			FileInfo fi = new(path);
			Profile? p = null;
			if (fi.Exists)
			{
				p = Profiles.FirstOrDefault(p => string.Compare(p.Name, fi.Name) == 0);
				if (p == null)
				{
					int id = (Profiles.Count > 0 ? Profiles.Max(p => p.ID) : 0) + 1;
					p = new Profile(fi, id);
					Profiles.Add(p);
					Save();
					return p;
				}
				return p;
			}

			p = Profiles.FirstOrDefault(p => string.Compare(p.Name, path) == 0 || string.Compare(p.Url, path) == 0);
			if (p == null)
			{
				int id = (Profiles.Count > 0 ? Profiles.Max(p => p.ID) : 0) + 1;
				p = new Profile(path, id);
				Profiles.Add(p);
				Save();
				return p;
			}
			return p;
		}

	}
}
