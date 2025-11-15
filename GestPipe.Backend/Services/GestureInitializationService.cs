using GestPipe.Backend.Models;
using GestPipe.Backend.Services.IServices;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GestPipe.Backend.Services
{
    public class GestureInitializationService : IGestureInitializationService
    {
        private readonly IMongoCollection<DefaultGesture> _defaultGestures;
        private readonly IMongoCollection<UserGestureConfig> _userGestureConfigs;
        private readonly ILogger<GestureInitializationService> _logger;

        public GestureInitializationService(
            IMongoDatabase database,
            ILogger<GestureInitializationService> logger)
        {
            _defaultGestures = database.GetCollection<DefaultGesture>("DefaultGestures");
            _userGestureConfigs = database.GetCollection<UserGestureConfig>("UserGestureConfigs"); 
            _logger = logger;
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

        public async Task<bool> InitializeUserGesturesAsync(string userId)
        {
            try
            {
                _logger.LogInformation("🚀 Starting gesture initialization for user: {UserId}", userId);

                // Step 1: Check if user already has gestures
                var hasGestures = await HasUserGesturesAsync(userId);
                if (hasGestures)
                {
                    _logger.LogInformation("⏭️ User already has gestures. Skipping initialization: UserId={UserId}", userId);
                    return true;
                }

                // Step 2: Get default gestures
                _logger.LogInformation("📥 Fetching default gestures...");
                var filter = Builders<DefaultGesture>.Filter.Eq("status.en", "Active");

                var defaultGestures = await _defaultGestures
                    .Find(filter)
                    .ToListAsync();

                _logger.LogInformation("📦 Found {Count} default gestures to copy", defaultGestures.Count);

                if (!defaultGestures.Any())
                {
                    _logger.LogWarning("⚠️ No active default gestures found!");

                    var totalCount = await _defaultGestures.CountDocumentsAsync(Builders<DefaultGesture>.Filter.Empty);
                    _logger.LogWarning("📊 Total gestures in DefaultGestures collection: {TotalCount}", totalCount);

                    return false;
                }

                // Step 3: Create user gesture configs
                _logger.LogInformation("🔨 Creating {Count} user gesture configs...", defaultGestures.Count);

                var userGestures = new List<UserGestureConfig>();
                var now = DateTime.UtcNow;

                foreach (var defaultGesture in defaultGestures)
                {
                    _logger.LogDebug("➕ Adding gesture: {PoseLabel} for user: {UserId}",
                        defaultGesture.PoseLabel, userId);

                    var userGesture = new UserGestureConfig
                    {
                        UserId = userId,
                        GestureTypeId = defaultGesture.GestureTypeId,
                        PoseLabel = defaultGesture.PoseLabel,
                        VectorData = new VectorData
                        {
                            Fingers = (int[])defaultGesture.VectorData.Fingers.Clone(),
                            MainAxisX = defaultGesture.VectorData.MainAxisX,
                            MainAxisY = defaultGesture.VectorData.MainAxisY,
                            DeltaX = defaultGesture.VectorData.DeltaX,
                            DeltaY = defaultGesture.VectorData.DeltaY
                        },
                        Accuracy = defaultGesture.Accuracy,
                        Status = new Dictionary<string, string>
                        {
                            { "en", "Active" },
                            { "vi", "Đã kích hoạt" }
                        },
                        UpdateAt = now
                    };

                    userGestures.Add(userGesture);
                }

                // Step 4: Insert to database
                _logger.LogInformation("💾 Inserting {Count} gestures to database...", userGestures.Count);

                await _userGestureConfigs.InsertManyAsync(userGestures);

                _logger.LogInformation("✅ Successfully inserted gestures to database");

                // Step 5: Verify insertion
                _logger.LogInformation("🔍 Verifying insertion...");
                var verifyCount = await _userGestureConfigs
                    .CountDocumentsAsync(u => u.UserId == userId);

                _logger.LogInformation("✅ Verification: Found {VerifyCount} gestures for user: {UserId}",
                    verifyCount, userId);

                if (verifyCount != userGestures.Count)
                {
                    _logger.LogError("❌ MISMATCH! Created {Created} but found {Found} in database",
                        userGestures.Count, verifyCount);
                    return false;
                }

                _logger.LogInformation("🎉 Successfully created {Count} gestures for user: {UserId}",
                    userGestures.Count, userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error initializing gestures for user: {UserId}, Message: {Message}, StackTrace: {StackTrace}",
                    userId, ex.Message, ex.StackTrace);
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
    }
}