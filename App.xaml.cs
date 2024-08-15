using iptv_manager_maui.Webhost;

namespace iptv_manager_maui
{
	public partial class App : Application
	{

		private readonly IEmbedIOService _embedIOService;

		public App(IEmbedIOService embedIOService)
		{
			InitializeComponent();

			MainPage = new AppShell();
			_embedIOService = embedIOService;
		}

		protected override void OnStart()
		{
			base.OnStart();
			_embedIOService.StartServer();
		}

		protected override void OnSleep()
		{
			base.OnSleep();
			_embedIOService.StopServer();
		}

		protected override void OnResume()
		{
			base.OnResume();
			_embedIOService.StartServer();
		}
	}
}
