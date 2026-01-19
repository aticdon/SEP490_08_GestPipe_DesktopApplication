using GestPipe.Backend.Models;
using GestPipe.Backend.Services.IServices;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services
{
    public class GestureInitializationService : IGestureInitializationService
    {
        private readonly IMongoCollection<DefaultGesture> _defaultGestures;
        private readonly IMongoCollection<UserGestureConfig> _userGestureConfigs;
        private readonly ILogger<GestureInitializationService> _logger;
        private readonly string pythonExe;
        private readonly string scriptPath;

        public GestureInitializationService(
            IMongoDatabase database,
            ILogger<GestureInitializationService> logger)
        {
            _defaultGestures = database.GetCollection<DefaultGesture>("DefaultGestures");
            _userGestureConfigs = database.GetCollection<UserGestureConfig>("UserGestureConfigs"); 
            _logger = logger;
            pythonExe = @"C:\Users\Admin\AppData\Local\Programs\Python\Python311\python.exe";
            scriptPath = @"D:\Semester9\codepython\hybrid_realtime_pipeline\code\create_user_folder.py";
        }

        public async Task<bool> HasUserGesturesAsync(string userId)
        {
            try
            {
                _logger.LogInformation("🔍 Checking if user has gestures: {UserId}", userId);

                var count = await _userGestureConfigs
                    .CountDocumentsAsync(u => u.UserId == userId);

                _logger.LogInformation("📊 Found {Count} gestures for user: {UserId}", count, userId);
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error checking if user has gestures: UserId={UserId}", userId);
                return false;
            }
        }
        public async Task<GestureStats> GetUserGestureStatsAsync(string userId)
        {
            try
            {
                var userGestures = await _userGestureConfigs
                    .Find(u => u.UserId == userId)
                    .ToListAsync();

                var activeCount = userGestures.Count(g => g.Status.ContainsKey("en") && g.Status["en"] == "Active");
                var inactiveCount = userGestures.Count - activeCount;

                return new GestureStats
                {
                    TotalGestures = userGestures.Count,
                    ActiveGestures = activeCount,
                    InactiveGestures = inactiveCount,
                    AverageAccuracy = userGestures.Any() ? userGestures.Average(g => g.Accuracy) : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gesture stats for user: {UserId}", userId);
                return new GestureStats();
            }
        }
        //public async Task<bool> CreateUserDataFolderAsync(string userId)
        //{
        //    try
        //    {
        //        _logger.LogInformation("[DRIVE_SYNC_DEFAULTS] Start for {UserId}", userId);

        //        // ⭐ LẤY DEFAULTS TRÊN DRIVE VỀ user_<id>
        //        await _googleDriveService.SyncUserFolderFromDefaultsAsync(userId);

        //        _logger.LogInformation("[DRIVE_SYNC_DEFAULTS] Completed for {UserId}", userId);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error syncing defaults from Drive for {UserId}", userId);
        //        return false;
        //    }
        //}

        public async Task<bool> CreateUserDataFolderAsync(string userId)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = pythonExe,
                    Arguments = $"\"{scriptPath}\" {userId}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var process = Process.Start(psi);
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                _logger.LogInformation("[PYTHON_FOLDER_CREATE] Output: {Output}, Error: {Error}", output, error);
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user data folder for {UserId}", userId);
                return false;
            }
        }
    }
}