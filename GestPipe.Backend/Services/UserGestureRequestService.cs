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
                    && x.Status["main"] == "pending")
                .ToListAsync();
        }

        public async Task<UserGestureRequest> GetByIdAsync(string id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<UserGestureRequest> GetLatestRequestByConfigIdAsync(string configId)
        {
            return await _collection
                .Find(x => x.UserGestureConfigId == configId)
                .SortByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public bool SetPendingToTraining(string requestId)
        {
            var filter = Builders<UserGestureRequest>.Filter.Eq(r => r.UserGestureConfigId, requestId);
            var update = Builders<UserGestureRequest>.Update
                .Set("status.en", "Training")
                .Set("status.vi", "Huấn luyện");
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
