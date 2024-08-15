using System.Text.Json;

namespace iptv_manager_maui.Sources.Utils
{
	internal class IpTvConfig
	{
		static string jsonKey = "profiles";

		public static string GetM3UFilePath(string fileName)
		{
			string dataDirectory = FileSystem.AppDataDirectory;

			string filePath = Path.Combine(dataDirectory,fileName);

			return filePath;
		}

		static IpTvConfig? _instance;
		public static IpTvConfig Instance
		{
			get
			{
				if (_instance == null)
				{
					try
					{
						var jsonData = Preferences.Get("iptv_config", "{}");
						_instance = JsonSerializer.Deserialize<IpTvConfig>(jsonData);
						if (_instance == null)
						{
							_instance = new IpTvConfig();
						}
					}
					catch
					{
						_instance = new IpTvConfig();
					}
				}
				return _instance;
			}
		}

		public void Save()
		{
			Preferences.Set("iptv_config", JsonSerializer.Serialize(this));
		}

	}
}
