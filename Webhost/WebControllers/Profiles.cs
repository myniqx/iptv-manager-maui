using Downloader;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using iptv_manager_maui.Sources.Database;
using iptv_manager_maui.Sources.Database.Enums;
using iptv_manager_maui.Sources.Database.Models;
using iptv_manager_maui.Sources.Profiles;
using iptv_manager_maui.Sources.Utils;
using System.Text.Json;

namespace iptv_manager_maui.Webhost.WebControllers
{
	public sealed class Profiles : WebApiController
	{

		[Route(HttpVerbs.Post, "/addurl")]
		public bool AddUrl([JsonData] AddUrlParam param)
		{
			var url = param.Url ?? "";
			if (Helpers.IsValidUrl(url) == false)
				return false;

			using (var _context = new M3UDatabase())
			{
				int profileCount = _context.Profiles.Max(p => p.ID);
				string profileName = $"Profile#{profileCount + 1}";

				var baseGroup = new Group
				{
					ProfileID = 0,
					GroupType = EGroupType.Default,
					Status = EStatus.None,
					LastUpdate = DateTime.Now
				};

				var profile = new Profile
				{
					Url = url,
					Name = profileName,
					BaseGroup = baseGroup,
					CreatedDate = DateTime.Now,
				};

				_context.Profiles.Add(profile);
				_context.SaveChanges();

				baseGroup.ProfileID = profile.ID;
				_context.Groups.Add(baseGroup);
				_context.SaveChanges();
			}

			return true;
		}

		[Route(HttpVerbs.Get, "/getAll")]
		public string GetProfiles()
		{
			using (var _context = new M3UDatabase())
			{
				var profiles = _context.Profiles.ToList();
				return JsonSerializer.Serialize(profiles);
			}
		}

		[Route(HttpVerbs.Get, "/get/{id}")]
		public string GetProfile(string id)
		{
			using (var _context = new M3UDatabase())
			{
				var profile = _context.Profiles
					.Where(p => p.ID == int.Parse(id))
					.FirstOrDefault();
				return JsonSerializer.Serialize(profile);
			}
		}

		private CancellationTokenSource _globalCts = new();

		[Route(HttpVerbs.Get, "/beat")]
		public async Task Beat()
		{
			var response = HttpContext.Response;
			response.ContentType = "text/event-stream";
			response.Headers.Set("Cache-Control", "no-cache");
			response.Headers.Set("Connection", "keep-alive");

			using var localCts = CancellationTokenSource.CreateLinkedTokenSource(_globalCts.Token, HttpContext.CancellationToken);

			try
			{
				while (!localCts.IsCancellationRequested)
				{
					var heartbeat = ProfileManager.Instance.GetHeartbeat();
					if (heartbeat == null || heartbeat.IsCompleted)
					{
						break; // İşlem tamamlandı
					}

					var data = $"data: {JsonSerializer.Serialize(heartbeat)}\n\n";

					try
					{
						await response.OutputStream.WriteAsync(
							System.Text.Encoding.UTF8.GetBytes(data),
							0,
							data.Length,
							localCts.Token);
						await response.OutputStream.FlushAsync(localCts.Token);
					}
					catch (IOException)
					{
						// Bağlantı koptu
						break;
					}

					await Task.Delay(300, localCts.Token);
				}
			}
			catch (OperationCanceledException)
			{
				// İstek iptal edildi veya bağlantı kapatıldı
			}
		}

		public void Dispose()
		{
			_globalCts.Cancel();
			_globalCts.Dispose();
		}

		[Route(HttpVerbs.Get, "/update/{id}")]
		public async Task UpdateProfile(string id)
		{
			var response = HttpContext.Response;
			response.ContentType = "text/event-stream";
			response.Headers.Set("Cache-Control", "no-cache");
			response.Headers.Set("Connection", "keep-alive");

			using var localCts = CancellationTokenSource.CreateLinkedTokenSource(_globalCts.Token, HttpContext.CancellationToken);

			try
			{
				// Profil ID'sini alın
				int profileId = int.Parse(id);

				// Profil veritabanından alın
				Profile? profile = null;
				using (var _context = new M3UDatabase())
				{
					profile = _context.Profiles.FirstOrDefault(p => p.ID == profileId);
				}

				if (profile == null)
				{
					// Profil bulunamadı hatası gönder
					await SendError(response, localCts.Token, "Profil bulunamadı.");
					return;
				}

				// İndirme işlemini başlat
				var downloadService = new DownloadService();
				downloadService.DownloadProgressChanged += async (sender, e) =>
				{
					// İndirme ilerlemesini cliente gönder
					var progressData = new
					{
						ProgressPercentage = e.ProgressPercentage,
						ReceivedBytesSize = e.ReceivedBytesSize,
						TotalBytesToReceive = e.TotalBytesToReceive,
						BytesPerSecondSpeed = e.BytesPerSecondSpeed
					};

					await SendProgress(response, localCts.Token, progressData);
				};

				downloadService.DownloadFileCompleted += async (sender, e) =>
				{
					// İndirme tamamlandı mesajı gönder
					await SendSuccess(response, localCts.Token);
				};

				// Dosyayı indir
				var filePath = IpTvConfig.GetM3UFilePath(profile.M3UFile);
				await downloadService.DownloadFileTaskAsync(profile.Url, filePath);
			}
			catch (Exception ex)
			{
				// Hata mesajı gönder
				await SendError(response, localCts.Token, ex.Message);
			}
		}

		private async Task SendProgress(IHttpResponse response, CancellationToken token, object data)
		{
			var message = $"data: {JsonSerializer.Serialize(data)}\n\n";
			await response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(message), 0, message.Length, token);
			await response.OutputStream.FlushAsync(token);
		}

		private async Task SendSuccess(IHttpResponse response, CancellationToken token)
		{
			var message = $"data: {{ \"status\": \"success\" }}\n\n";
			await response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(message), 0, message.Length, token);
			await response.OutputStream.FlushAsync(token);
		}

		private async Task SendError(IHttpResponse response, CancellationToken token, string errorMessage)
		{
			var message = $"data: {{ \"status\": \"error\", \"message\": \"{errorMessage}\" }}\n\n";
			await response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(message), 0, message.Length, token);
			await response.OutputStream.FlushAsync(token);
		}
	}

	public class AddUrlParam
	{
		public string? Url { get; set; }
	}
}
