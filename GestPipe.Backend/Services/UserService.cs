using GestPipe.Backend.Config;
using GestPipe.Backend.Models;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.IServices;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace GestPipe.Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly I18nConfig _i18nConfig;

        public UserService(IOptions<MongoDbSettings> mongoSettings, IOptions<I18nConfig> i18nOptions = null)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users");

            _i18nConfig = i18nOptions?.Value ?? new I18nConfig { SupportedCultures = new List<string> { "en-US" }, DefaultCulture = "en-US" };
        }

        public List<User> GetAll() => _users.Find(user => true).ToList();

        public User GetById(string id) => _users.Find(user => user.Id == id).FirstOrDefault();

        public void Create(User user) => _users.InsertOne(user);

        // Set language for a user; returns true if update success
        public bool SetLanguage(string userId, string language)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(language))
                return false;

            var supported = _i18nConfig?.SupportedCultures ?? new List<string> { "en-US" };
            if (!supported.Contains(language))
                return false;

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.UiLanguage, language);
            var res = _users.UpdateOne(filter, update);
            return res.ModifiedCount > 0;
        }

        public bool IncrementRequestCount(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update
                .Inc(u => u.RequestCountToday, 1);
            var res = _users.UpdateOne(filter, update);
            return res.ModifiedCount > 0;
        }

        public bool UpdateGestureRequestStatus(string userId, string status)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update
                .Set(u => u.GestureRequestStatus, status);
            var res = _users.UpdateOne(filter, update);
            return res.ModifiedCount > 0;
        }
        public bool SetUseCustomModel(string userId, bool useCustomModel)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.UseCustomModel, useCustomModel);
            var res = _users.UpdateOne(filter, update);
            return res.ModifiedCount > 0;
        }
    }
}