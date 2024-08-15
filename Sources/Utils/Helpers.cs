namespace iptv_manager_maui.Sources.Utils
{
	internal class Helpers
	{
		public static bool IsValidUrl(string url)
		{
			return Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttp;
		}
	}
}
