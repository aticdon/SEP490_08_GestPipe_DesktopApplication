using GestPipe.GestPipePowerPoint.Models.DTOs;
using GestPipePowerPonit.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Services
{
    public class GestureDownloadService
    {
        private readonly UserGestureConfigService _uGestureService;
        private readonly UserService _userService;
        private readonly HttpClient _httpClient;

        // TODO: nếu sau này muốn đẹp hơn, kéo path này ra config
        private readonly string _pythonBasePath =
            @"D:\Semester9\codepython\hybrid_realtime_pipeline\code";

        public GestureDownloadService(
            UserGestureConfigService uGestureService,
            UserService userService)
        {
            _uGestureService = uGestureService;
            _userService = userService;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7219/")
            };
        }
        /// <summary>
        /// Gọi API sync-user (start job) và liên tục gọi API progress để báo tiến độ.
        /// </summary>
        /// <param name="userId">Id user</param>
        /// <param name="onProgress">
        /// Callback được gọi mỗi lần lấy được tiến độ mới: (syncedFiles, totalFiles).
        /// </param>
        public async Task SyncFromDriveAsync(string userId, Action<int, int> onProgress)
        {
            // 1. Start sync job
            var startResp = await _httpClient.PostAsync($"/api/drivesync/sync-user/{userId}", null);
            var startJson = await startResp.Content.ReadAsStringAsync();
            Console.WriteLine("[SyncFromDriveAsync] start JSON = " + startJson);

            if (!startResp.IsSuccessStatusCode)
                throw new Exception($"Sync Google Drive failed: {startResp.StatusCode} - {startJson}");

            // 2. Poll progress cho tới khi IsCompleted = true
            while (true)
            {
                var progResp = await _httpClient.GetAsync($"/api/drivesync/sync-user/progress/{userId}");
                var progJson = await progResp.Content.ReadAsStringAsync();
                Console.WriteLine("[SyncFromDriveAsync] progress JSON = " + progJson);

                if (!progResp.IsSuccessStatusCode)
                    throw new Exception($"Get sync progress failed: {progResp.StatusCode} - {progJson}");

                var progress = JsonConvert.DeserializeObject<DriveSyncProgress>(progJson);
                if (progress == null)
                    throw new Exception("Cannot parse sync progress: " + progJson);

                // Gọi callback cập nhật UI
                onProgress?.Invoke(progress.SyncedFiles, progress.TotalFiles);

                if (progress.IsCompleted)
                    break;

                await Task.Delay(300); // 0.3 giây poll 1 lần
            }
        }



        /// <summary>
        /// Đọc CSV từ thư mục python và import vào UserGestureConfig.
        /// </summary>
        public async Task<int> ImportGesturesFromCsvAsync(string userId)
        {
            string userPath = $"user_{userId}";
            string csvPath = Path.Combine(
                _pythonBasePath,
                userPath,
                "training_results",
                "gesture_data_compact.csv"
            );

            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("CSV file not found", csvPath);
            }

            string csvContent = File.ReadAllText(csvPath, Encoding.UTF8);
            int inserted = await _uGestureService.ImportFromCsvAsync(userId, csvContent);
            return inserted;
        }

        /// <summary>
        /// Enable lại trạng thái request gesture cho user sau khi import xong.
        /// </summary>
        public async Task EnableGestureRequestAsync(string userId)
        {
            var success = await _userService.UpdateGestureRequestStatusAsync(userId, "enabled");
            if (!success)
            {
                Console.WriteLine("[GestureDownloadService] Warning: UpdateGestureRequestStatusAsync returned false.");
            }
        }
    }
}
