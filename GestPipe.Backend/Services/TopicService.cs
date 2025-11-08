using GestPipe.Backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;

namespace GestPipe.Backend.Services
{
    public class TopicService
    {
        private readonly IMongoCollection<Topic> _topics;

        public TopicService(IOptions<MongoDBSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.Database);
            _topics = database.GetCollection<Topic>("Topic");
        }

        public List<Topic> GetAll() => _topics.Find(topic => true).ToList();

        public Topic Get(string id) => _topics.Find(topic => topic.Id == id).FirstOrDefault();

        public void Create(Topic topic) => _topics.InsertOne(topic);
    }
}