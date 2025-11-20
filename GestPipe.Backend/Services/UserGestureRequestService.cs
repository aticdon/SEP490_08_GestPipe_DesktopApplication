using GestPipe.Backend.Models;
using GestPipe.Backend.Services.IServices;
using MongoDB.Driver;

namespace GestPipe.Backend.Services
{
    public class UserGestureRequestService : IUserGestureRequestService
    {
        private readonly IMongoCollection<UserGestureRequest> _collection;

        public UserGestureRequestService(IMongoDatabase db)
        {
            _collection = db.GetCollection<UserGestureRequest>("UserGestureRequests");
        }

        public async Task<UserGestureRequest> CreateRequestAsync(UserGestureRequest request)
        {
            await _collection.InsertOneAsync(request);
            return request;
        }

        public async Task<List<UserGestureRequest>> GetPendingRequestsByUserAsync(string userId)
        {
            return await _collection
                .Find(x => x.UserId == userId
                    && x.Status != null
                    && x.Status.ContainsKey("main")
                    && x.Status["main"] == "customed")
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
        public bool SetPendingToTraining(string requestId)
        {
            var filter = Builders<UserGestureRequest>.Filter.Eq(r => r.UserGestureConfigId, requestId);
            var update = Builders<UserGestureRequest>.Update
                .Set("status.en", "Submit")
                .Set("status.vi", "Gửi");
            var res = _collection.UpdateOne(filter, update);
            return res.ModifiedCount > 0;
        }

        public bool SetTrainingToSuccessful(string requestId)
        {
            var filter = Builders<UserGestureRequest>.Filter.Eq(r => r.UserGestureConfigId, requestId);
            var update = Builders<UserGestureRequest>.Update
                .Set("status.en", "Successful")
                .Set("status.vi", "Thành công");
            var res = _collection.UpdateOne(filter, update);
            return res.ModifiedCount > 0;
        }
    }
}
