using GestPipe.Backend.Models;
using GestPipe.Backend.Services.IServices;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
namespace GestPipe.Backend.Services
{
    public class UserGestureRequestService : IUserGestureRequestService
    {
        private readonly IMongoCollection<UserGestureRequest> _collection;

        public UserGestureRequestService(IMongoDatabase db)
        {
            _collection = db.GetCollection<UserGestureRequest>("UserGestureRequests");
        }

        //public async Task<UserGestureRequest> CreateRequestAsync(UserGestureRequest request)
        //{
        //    await _collection.InsertOneAsync(request);
        //    return request;
        //}
        public async Task<UserGestureRequest> CreateRequestAsync(UserGestureRequest request)
        {
            // ✅ TC4: Validate request object not null
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request object cannot be null.");
            }

            // ✅ TC6, TC7: Validate UserId not null/empty
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                throw new ArgumentException("UserId is required.", nameof(request.UserId));
            }

            // ✅ TC8: Validate UserId is valid ObjectId
            if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out _))
            {
                throw new ArgumentException("UserId must be a valid ObjectId.", nameof(request.UserId));
            }

            // ✅ TC9, TC10: Validate GestureTypeId not null/empty
            if (string.IsNullOrWhiteSpace(request.GestureTypeId))
            {
                throw new ArgumentException("GestureTypeId is required.", nameof(request.GestureTypeId));
            }

            // ✅ TC11, TC12: Validate UserGestureConfigId not null/empty
            if (string.IsNullOrWhiteSpace(request.UserGestureConfigId))
            {
                throw new ArgumentException("UserGestureConfigId is required.", nameof(request.UserGestureConfigId));
            }

            // ✅ TC13, TC14: Validate PoseLabel (nếu yêu cầu bắt buộc)
            if (string.IsNullOrWhiteSpace(request.PoseLabel))
            {
                throw new ArgumentException("PoseLabel is required.", nameof(request.PoseLabel));
            }

            // ✅ TC15, TC16: Validate Status not null/empty
            if (request.Status == null || request.Status.Count == 0)
            {
                throw new ArgumentException("Status is required.", nameof(request.Status));
            }

            // ✅ TC2: Check all fields are not all null (already validated above)
            // ✅ TC3: Check all fields are not all empty (already validated above)

            // Set default values
            if (string.IsNullOrEmpty(request.Id))
            {
                request.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            }

            if (request.CreatedAt == default)
            {
                request.CreatedAt = DateTime.UtcNow;
            }

            // ✅ TC5, TC17: Database operations with error handling
            try
            {
                await _collection.InsertOneAsync(request);

                // ✅ TC1: Success
                return request;
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // ✅ TC17: Duplicate key error
                throw new InvalidOperationException($"A request with ID '{request.Id}' already exists.", ex);
            }
            catch (MongoException ex)
            {
                // ✅ TC5: MongoDB error
                throw new InvalidOperationException($"Database error while creating request: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // ✅ TC5: Generic error
                throw new InvalidOperationException($"Failed to create request: {ex.Message}", ex);
            }
        }

        public async Task<List<UserGestureRequest>> GetPendingRequestsByUserAsync(string userId)
        {
            return await _collection
                .Find(x => x.UserId == userId
                    && x.Status != null
                    && x.Status.ContainsKey("main")
                    && x.Status["main"] == "Customed")
                .ToListAsync();
        }

        public async Task<UserGestureRequest> GetByIdAsync(string id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<UserGestureRequest> GetLatestRequestByConfigIdAsync(string configId, string userId)
        {
            //return await _collection
            //    .Find(x => x.UserGestureConfigId == configId && x.UserId = userId)
            //    .SortByDescending(x => x.CreatedAt)
            //    .FirstOrDefaultAsync();
            return await _collection
                .Find(x => x.UserGestureConfigId == configId && x.UserId == userId)
                .SortByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
                }

        public async Task<List<UserGestureRequest>> GetLatestRequestsByConfigIdsAsync(List<string> configIds, string userId)
        {
            var filter = Builders<UserGestureRequest>.Filter.In(x => x.UserGestureConfigId, configIds)
                & Builders<UserGestureRequest>.Filter.Eq(x => x.UserId, userId);

            var requests = await _collection.Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();

            // Group mỗi config lấy request mới nhất
            var latestByConfig = requests
                .GroupBy(r => r.UserGestureConfigId)
                .Select(g => g.First())
                .ToList();

            return latestByConfig;
        }
        // ✅ THAY ĐỔI 1: SetPendingToTraining
        public bool SetPendingToTraining(string userGestureConfigId, string userId)
        {
            try
            {
                Console.WriteLine($"[SetPendingToTraining] Updating configId: {userGestureConfigId}, userId: {userId}");

                // ✅ Filter cả UserGestureConfigId VÀ UserId
                var filter = Builders<UserGestureRequest>.Filter.And(
                    Builders<UserGestureRequest>.Filter.Eq(r => r.UserGestureConfigId, userGestureConfigId),
                    Builders<UserGestureRequest>.Filter.Eq(r => r.UserId, userId),
                    Builders<UserGestureRequest>.Filter.Or(
                    Builders<UserGestureRequest>.Filter.Eq("status.en", "Customed"),
                    Builders<UserGestureRequest>.Filter.Eq("status.vi", "Đã tùy chỉnh")
                    )
                );

                // Tìm request mới nhất của user này với config này
                var latestRequest = _collection
                    .Find(filter)
                    .SortByDescending(r => r.CreatedAt)
                    .FirstOrDefault();

                if (latestRequest == null)
                {
                    Console.WriteLine($"[SetPendingToTraining] ❌ No request found");
                    return false;
                }

                Console.WriteLine($"[SetPendingToTraining] ✅ Found request: {latestRequest.Id}");

                // Update theo Id cụ thể
                var updateFilter = Builders<UserGestureRequest>.Filter.Eq(r => r.Id, latestRequest.Id);
                var update = Builders<UserGestureRequest>.Update
                    .Set("status.en", "Submit")
                    .Set("status.vi", "Gửi");

                var res = _collection.UpdateOne(updateFilter, update);

                Console.WriteLine($"[SetPendingToTraining] ModifiedCount: {res.ModifiedCount}");

                return res.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SetPendingToTraining] ❌ Exception: {ex.Message}");
                return false;
            }
        }
        public bool SetTrainingToSuccessful(string userGestureConfigId, string userId)
        {
            try
            {
                Console.WriteLine($"[SetTrainingToSuccessful] Updating configId: {userGestureConfigId}, userId: {userId}");

                // ✅ Filter cả UserGestureConfigId VÀ UserId
                var filter = Builders<UserGestureRequest>.Filter.And(
                    Builders<UserGestureRequest>.Filter.Eq(r => r.UserGestureConfigId, userGestureConfigId),
                    Builders<UserGestureRequest>.Filter.Eq(r => r.UserId, userId),
                    Builders<UserGestureRequest>.Filter.Or(
                    Builders<UserGestureRequest>.Filter.Eq("status.en", "Submit"),
                    Builders<UserGestureRequest>.Filter.Eq("status.vi", "Gửi")
                    )
                );

                var latestRequest = _collection
                    .Find(filter)
                    .SortByDescending(r => r.CreatedAt)
                    .FirstOrDefault();

                if (latestRequest == null)
                {
                    Console.WriteLine($"[SetTrainingToSuccessful] ❌ No request found");
                    return false;
                }

                Console.WriteLine($"[SetTrainingToSuccessful] ✅ Found request: {latestRequest.Id}");

                var updateFilter = Builders<UserGestureRequest>.Filter.Eq(r => r.Id, latestRequest.Id);
                var update = Builders<UserGestureRequest>.Update
                    .Set("status.en", "Successful")
                    .Set("status.vi", "Thành công");

                var res = _collection.UpdateOne(updateFilter, update);

                Console.WriteLine($"[SetTrainingToSuccessful] ModifiedCount: {res.ModifiedCount}");

                return res.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SetTrainingToSuccessful] ❌ Exception: {ex.Message}");
                return false;
            }
        }
        public bool SetTrainingToCanceled(string userGestureConfigId, string userId)
        {
            try
            {
                var filter = Builders<UserGestureRequest>.Filter.And(
                    Builders<UserGestureRequest>.Filter.Eq(r => r.UserGestureConfigId, userGestureConfigId),
                    Builders<UserGestureRequest>.Filter.Eq(r => r.UserId, userId),
                    Builders<UserGestureRequest>.Filter.Or(
                    Builders<UserGestureRequest>.Filter.Eq("status.en", "Customed"),
                    Builders<UserGestureRequest>.Filter.Eq("status.vi", "Đã tùy chỉnh")
                    )
                );

                var latestRequest = _collection
                    .Find(filter)
                    .SortByDescending(r => r.CreatedAt)
                    .FirstOrDefault();

                if (latestRequest == null)
                {
                    Console.WriteLine($"[SetTrainingToSuccessful] ❌ No request found");
                    return false;
                }

                Console.WriteLine($"[SetTrainingToSuccessful] ✅ Found request: {latestRequest.Id}");

                var updateFilter = Builders<UserGestureRequest>.Filter.Eq(r => r.Id, latestRequest.Id);
                var update = Builders<UserGestureRequest>.Update
                    .Set("status.en", "Canceled")
                    .Set("status.vi", "Đã hủy");

                var res = _collection.UpdateOne(updateFilter, update);

                Console.WriteLine($"[SetTrainingToSuccessful] ModifiedCount: {res.ModifiedCount}");

                return res.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SetTrainingToSuccessful] ❌ Exception: {ex.Message}");
                return false;
            }
        }
    }
}
