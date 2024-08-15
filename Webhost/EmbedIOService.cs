using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using iptv_manager_maui.Webhost.WebApis;
using iptv_manager_maui.Webhost.WebControllers;

namespace iptv_manager_maui.Webhost
{

	public interface IEmbedIOService
	{
		void StartServer();
		void StopServer();
	}

	public class EmbedIOService : IEmbedIOService
	{
		private readonly WebServer _server;

		public EmbedIOService()
		{
			_server = new WebServer(o => o
					.WithUrlPrefix("http://*:5000")
					.WithMode(HttpListenerMode.EmbedIO))
				.WithCors("*")
				.WithWebApi("/settings", m => m.WithController<Settings>())
				.WithWebApi("/profile", m => m.WithController<Profiles>())
				.WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));
		}

		public void StartServer()
		{
			Task.Run(() => _server.RunAsync());
		}

		public void StopServer()
		{
			_server.Dispose();
		}
	}

}

