using System.Diagnostics;
using System.Text.RegularExpressions;

namespace iptv_manager_maui.Sources.M3U
{

	public class M3UParser
	{
		private FileInfo _fileInfo { get; set; }

		long _fileSize;
		long _fileCursor;
		double _lastPercent;

		string line1st="",line2nd="";

		public M3UParser(FileInfo fi)
		{
			_fileInfo = fi;
			_fileSize = fi.Length;
			_lastPercent = 0;
		}

		public event EventHandler<double>? ProgressChanged;

		// Regex for parsing #EXTINF lines
		private static readonly Regex _extinfRegex = new(@"^#EXTINF:-?\d+(?:\s+(.+?))?\s*,\s*(.*)$", RegexOptions.Compiled);

		// Regex for parsing key-value pairs
		private static readonly Regex _keyValuePairRegex = new(@"^(\w+)=(""[^\""]+"")|([^\""]+)$", RegexOptions.Compiled);

		public async Task<List<M3UObject>> LoadM3U()
		{
			// Preallocate the list with an estimated capacity
			List<M3UObject> objects = new(50000);

			try
			{
				using (var fileStream = _fileInfo.OpenRead())
				using (var buffered = new BufferedStream(fileStream))
				using (var reader = new StreamReader(buffered))
				{
					if (!await readHeader(reader).ConfigureAwait(false))
						return objects;

					while (await readElement(reader).ConfigureAwait(false))
					{
						var match = _extinfRegex.Match(line1st);
						if (match.Success)
						{
							var duration = match.Groups[1].Value; // Not used in your code, but available
							var name = match.Groups[2].Value;
							var url = line2nd;

							var m3uObject = new M3UObject
							{
								TvgName = name,
								UrlTvg = url
							};

							// Parse key-value pairs from the #EXTINF line
							var attributes = match.Groups[1].Value;
							if (!string.IsNullOrEmpty(attributes))
							{
								foreach (Match kvpMatch in _keyValuePairRegex.Matches(attributes))
								{
									var key = kvpMatch.Groups[1].Value.ToLower();
									var value = kvpMatch.Groups[2].Success ? kvpMatch.Groups[2].Value.Trim('"') : kvpMatch.Groups[3].Value;

									switch (key)
									{
										case "tvg-logo":
											m3uObject.TvgLogo = value;
											break;
										case "group-title":
											m3uObject.GroupTitle = value;
											break;
											// Add more cases for other attributes as needed
									}
								}
							}

							objects.Add(m3uObject);
						}
						else
						{
							Debug.WriteLine($"Error parsing with;{Environment.NewLine}{line1st}{Environment.NewLine}{line2nd}");
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Parsing error : {e.Message}");
			}

			return objects;
		}

		private async Task<bool> readElement(StreamReader _stream)
		{
			var line = await _stream.ReadLineAsync().ConfigureAwait(false);
			if (line == null)
				return false;
			line1st = line;
			line = await _stream.ReadLineAsync().ConfigureAwait(false);
			if (line == null)
				return false;
			line2nd = line;
			_fileCursor += line1st.Length + line2nd.Length + 2;
			double percent = ((double)_fileCursor / _fileSize) * 100.0;
			if (percent - _lastPercent > 1.123)
			{
				_lastPercent = percent;
				ProgressChanged?.Invoke(this, _lastPercent);
			}
			return true;
		}

		private async Task<bool> readHeader(StreamReader _stream)
		{
			var head = await _stream.ReadLineAsync().ConfigureAwait(false);
			if (head == null)
				return false;
			_fileCursor += head.Length + 1;
			return head.StartsWith("#EXTM3U", StringComparison.OrdinalIgnoreCase);
		}
	}
}

