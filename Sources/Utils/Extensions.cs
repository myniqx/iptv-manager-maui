namespace iptv_manager_maui.Sources.Utils
{
	internal static class Extensions
	{
		public static string toSize(this long value)
		{
			if (value < 1024)
				return $"{value} bytes";
			value >>= 10;
			if (value < 1128)
				return $"{value} kb";
			double bs = value / 1024.0;
			if (bs < 1128.0)
				return $"{bs:f2} mb";
			bs /= 1024.0f;
			return $"{bs:f2} gb";
		}

		public static string toSize(this double value) => ((long)value).toSize();

		public static string ReplaceInvalidFileNameCharactersWithSpace(this string input)
		{
			string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			return new string(input.Select(c => invalidChars.Contains(c) ? ' ' : c).ToArray()).Trim();
		}
	}
}
