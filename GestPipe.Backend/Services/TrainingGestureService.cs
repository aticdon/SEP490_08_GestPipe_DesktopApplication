using GestPipe.Backend.Models;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.IServices;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GestPipe.Backend.Services
{
    public class TrainingGestureService : ITrainingGestureService
    {
        private readonly IMongoCollection<TrainingGesture> _collection;

        public TrainingGestureService(IOptions<MongoDbSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _collection = database.GetCollection<TrainingGesture>("TrainingGesture");
        }

        public List<TrainingGesture> GetAll() => _collection.Find(x => true).ToList();

        public TrainingGesture Get(string id)
        {
            // Kiểm tra ObjectId hợp lệ nếu id dạng string (nâng cao)
            return _collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public void Create(TrainingGesture trainingGesture)
        {
            if (trainingGesture.Id == null || trainingGesture.Id == MongoDB.Bson.BsonString.Empty)
            {
                // Không gán gì để MongoDB tự sinh, hoặc gán GenerateNewId()
                // trainingGesture.Id = MongoDB.Bson.ObjectId.GenerateNewId();
            }
            trainingGesture.CreateAt = DateTime.Now;
            _collection.InsertOne(trainingGesture);
        }
    }
}
