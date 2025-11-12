using GestPipe.Backend.Models;
using GestPipe.Backend.Services.IServices;
using MongoDB.Driver;

namespace GestPipe.Backend.Services
{
    public class UserGestureConfigService : IUserGestureConfigService
    {
        private readonly IMongoCollection<UserGestureConfig> _userGestureConfigs;
        public UserGestureConfigService(IMongoDatabase database)
        {
            _userGestureConfigs = database.GetCollection<UserGestureConfig>("UserGestureConfigs");
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
    }
}
