using GestPipePowerPonit.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Services
{
    public class GestureUploadService
    {
        private readonly string _baseUrl;

        public GestureUploadService(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        /// <summary>
        /// Bắt đầu upload folder user_{userId} lên Drive và poll tiến độ.
        /// Trả về true nếu có file để upload, false nếu 0 file.
        /// </summary>
        public async Task<bool> UploadUserGesturesWithProgressAsync(
            string userId,
            Action<int, int> progressCallback,
            int pollingIntervalMs = 500)
        {
            using var http = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };

            // 1. Gửi request start upload (backend chạy nền)
            var startResp = await http.PostAsync($"/api/drivesync/upload-user/{userId}", null);
            var startBody = await startResp.Content.ReadAsStringAsync();

            if (!startResp.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Cannot start upload. Status: {(int)startResp.StatusCode}. Body: {startBody}"
                );
            }

            // 2. Poll tiến độ giống download
            while (true)
            {
                await Task.Delay(pollingIntervalMs);

                var progressResp = await http.GetAsync($"/api/drivesync/upload-user/progress/{userId}");
                if (!progressResp.IsSuccessStatusCode)
                {
                    // có thể log sau, ở client tạm bỏ qua lần poll lỗi
                    continue;
                }

                var json = await progressResp.Content.ReadAsStringAsync();
                var progress = JsonConvert.DeserializeObject<DriveUploadProgress>(json);

                if (progress == null)
                {
                    continue;
                }

                // Báo cho UI
                progressCallback?.Invoke(progress.UploadedFiles, progress.TotalFiles);

                if (progress.IsCompleted)
                {
                    // true nếu thực sự có file, false nếu 0 file
                    return progress.TotalFiles > 0;
                }
            }
        }
    }
}
