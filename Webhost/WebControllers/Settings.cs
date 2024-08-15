using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System.Diagnostics;

namespace iptv_manager_maui.Webhost.WebApis
{

	public sealed class Settings : WebApiController
	{

		[Route(HttpVerbs.Post, "/set")]
		public bool Set([JsonData] SetParams setParams)
		{
			Debug.WriteLine("/set > " + setParams.Key + " " + setParams.Value);
			if (string.IsNullOrWhiteSpace(setParams.Key) == true)
				return false;

			if (string.IsNullOrWhiteSpace(setParams.Value) == true)
				Preferences.Remove(setParams.Key);

			else
				Preferences.Set(setParams.Key, setParams.Value);

			return true;
		}

		[Route(HttpVerbs.Post, "/get")]
		public string Get([JsonData] GetParams getParams)
		{
			Debug.WriteLine("/get > " + getParams.Key + " " + getParams.DefValue);
			if (string.IsNullOrWhiteSpace(getParams.Key) == true)
				return "";

			var defValue = string.IsNullOrWhiteSpace(getParams.DefValue) ? "" : getParams.DefValue;

			return Preferences.Get(getParams.Key, defValue);
		}
	}

	public class SetParams
	{
		public string Key { get; set; }
		public string Value { get; set; }

		public SetParams()
		{
			Key = "";
			Value = "";
		}
	}

	public class GetParams
	{
		public string Key { get; set; }
		public string DefValue { get; set; }

		public GetParams()
		{
			Key = "";
			DefValue = "";
		}
	}

}
