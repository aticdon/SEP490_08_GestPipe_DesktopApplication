using GestPipe.Backend.Models;
using GestPipe.Backend.Services.IServices;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Globalization;

namespace GestPipe.Backend.Services
{
    public class UserGestureConfigService : IUserGestureConfigService
    {
        private readonly IMongoCollection<UserGestureConfig> _userGestureConfigs;
        private readonly IMongoCollection<DefaultGesture> _defaultGestures;
        public UserGestureConfigService(IMongoDatabase database)
        {
            _userGestureConfigs = database.GetCollection<UserGestureConfig>("UserGestureConfigs");
            _defaultGestures = database.GetCollection<DefaultGesture>("DefaultGestures");
        }
        public List<UserGestureConfig> GetAllByUserId(string userId)
        {
            var filter = Builders<UserGestureConfig>.Filter.Eq(x => x.UserId, userId);
            return _userGestureConfigs.Find(filter).ToList();
        }

        // Lấy config theo Id
        public UserGestureConfig GetById(string id)
        {
            var filter = Builders<UserGestureConfig>.Filter.Eq(x => x.Id, id);
            return _userGestureConfigs.Find(filter).FirstOrDefault();
        }
        //public async Task<int> ImportFromCsvAsync(string userId, string csvContent)
        //{
        //    // 1. Load toàn bộ default gestures để map pose_label → gesture_type_id
        //    var defaults = await _defaultGestures.Find(Builders<DefaultGesture>.Filter.Empty)
        //                                         .ToListAsync();

        //    var poseToType = defaults.ToDictionary(
        //        g => g.PoseLabel,
        //        g => g.GestureTypeId,
        //        StringComparer.OrdinalIgnoreCase
        //    );

        //    // 2. Tách dòng CSV
        //    var lines = csvContent
        //        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (lines.Length <= 1)
        //        return 0; // chỉ có header

        //    var toInsert = new List<UserGestureConfig>();

        //    // header: instance_id,pose_label,left_finger_state_0,...,delta_y
        //    for (int i = 1; i < lines.Length; i++)
        //    {
        //        var line = lines[i].Trim();
        //        if (string.IsNullOrWhiteSpace(line))
        //            continue;

        //        var cols = line.Split(',');
        //        if (cols.Length < 16)
        //            continue; // thiếu cột → bỏ qua

        //        string poseLabel = cols[1];

        //        // Không tìm được pose trong default → bỏ qua
        //        if (!poseToType.TryGetValue(poseLabel, out var gestureTypeId))
        //            continue;

        //        // Finger states
        //        int[] fingers = new int[10];
        //        for (int f = 0; f < 5; f++)
        //        {
        //            fingers[f] = int.Parse(cols[2 + f]); // left_finger_state_0-4
        //            fingers[f + 5] = int.Parse(cols[7 + f]); // right_finger_state_0-4
        //        }

        //        // Motion
        //        double mainAxisX = double.Parse(cols[12], CultureInfo.InvariantCulture);
        //        double mainAxisY = double.Parse(cols[13], CultureInfo.InvariantCulture);
        //        double deltaX = double.Parse(cols[14], CultureInfo.InvariantCulture);
        //        double deltaY = double.Parse(cols[15], CultureInfo.InvariantCulture);

        //        var vector = new VectorData
        //        {
        //            Fingers = fingers,
        //            MainAxisX = mainAxisX,
        //            MainAxisY = mainAxisY,
        //            DeltaX = deltaX,
        //            DeltaY = deltaY
        //        };

        //        var config = new UserGestureConfig
        //        {
        //            Id = ObjectId.GenerateNewId().ToString(),
        //            UserId = userId,
        //            GestureTypeId = gestureTypeId,
        //            PoseLabel = poseLabel,
        //            Status = new Dictionary<string, string>
        //            {
        //                ["en"] = "Active",
        //                ["vi"] = "Đã kích hoạt"
        //            },
        //            Accuracy = 0.0,              // CSV không có accuracy, tuỳ bạn set
        //            UpdateAt = DateTime.UtcNow,
        //            VectorData = vector
        //        };

        //        toInsert.Add(config);
        //    }

        //    if (toInsert.Count > 0)
        //    {
        //        await _userGestureConfigs.InsertManyAsync(toInsert);
        //    }

        //    return toInsert.Count;
        //}
        public async Task<int> ImportFromCsvAsync(string userId, string csvContent)
        {
            // 1. Load toàn bộ default gestures để map pose_label → gesture_type_id
            var defaults = await _defaultGestures.Find(Builders<DefaultGesture>.Filter.Empty)
                                                 .ToListAsync();

            var poseToType = defaults.ToDictionary(
                g => g.PoseLabel,
                g => g.GestureTypeId,
                StringComparer.OrdinalIgnoreCase
            );

            // 2. Tách dòng CSV
            var lines = csvContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length <= 1)
                return 0; // chỉ có header

            var toInsert = new List<UserGestureConfig>();

            // header: instance_id,pose_label,left_finger_state_0,...,delta_y
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var cols = line.Split(',');
                if (cols.Length < 16)
                    continue; // thiếu cột → bỏ qua

                string poseLabel = cols[1];

                // Không tìm được pose trong default → bỏ qua
                if (!poseToType.TryGetValue(poseLabel, out var gestureTypeId))
                    continue;

                // Finger states
                int[] fingers = new int[10];
                for (int f = 0; f < 5; f++)
                {
                    fingers[f] = int.Parse(cols[2 + f]);      // left_finger_state_0-4
                    fingers[f + 5] = int.Parse(cols[7 + f]);  // right_finger_state_0-4
                }

                // Motion
                double mainAxisX = double.Parse(cols[12], CultureInfo.InvariantCulture);
                double mainAxisY = double.Parse(cols[13], CultureInfo.InvariantCulture);
                double deltaX = double.Parse(cols[14], CultureInfo.InvariantCulture);
                double deltaY = double.Parse(cols[15], CultureInfo.InvariantCulture);

                var vector = new VectorData
                {
                    Fingers = fingers,
                    MainAxisX = mainAxisX,
                    MainAxisY = mainAxisY,
                    DeltaX = deltaX,
                    DeltaY = deltaY
                };

                var config = new UserGestureConfig
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = userId,
                    GestureTypeId = gestureTypeId,
                    PoseLabel = poseLabel,
                    Status = new Dictionary<string, string>
                    {
                        ["en"] = "Active",
                        ["vi"] = "Đã kích hoạt"
                    },
                    Accuracy = 0.0,              // CSV không có accuracy, tạm để 0
                    UpdateAt = DateTime.UtcNow,
                    VectorData = vector
                };

                toInsert.Add(config);
            }

            if (toInsert.Count == 0)
                return 0;

            // 🔁 GHI ĐÈ: xóa config cũ của user, insert config mới
            var filter = Builders<UserGestureConfig>.Filter.Eq(x => x.UserId, userId);
            await _userGestureConfigs.DeleteManyAsync(filter);

            await _userGestureConfigs.InsertManyAsync(toInsert);

            return toInsert.Count;
        }
    }
}
