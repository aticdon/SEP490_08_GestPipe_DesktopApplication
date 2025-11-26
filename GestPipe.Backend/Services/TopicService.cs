using GestPipe.Backend.Models;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.IServices;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;

namespace GestPipe.Backend.Services
{
    public class TopicService : ITopicService
    {
        private readonly IMongoCollection<Topic> _topics;

        //public TopicService(IOptions<MongoDbSettings> mongoSettings)
        public TopicService(IMongoDatabase database)
        {
            //var client = new MongoClient(mongoSettings.Value.ConnectionString);
            //var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _topics = database.GetCollection<Topic>("Topic");
        }

        public List<Topic> GetAll() => _topics.Find(topic => true).ToList();
    }
}