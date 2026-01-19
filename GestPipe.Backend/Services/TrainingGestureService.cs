using GestPipe.Backend.Models;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.IServices;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using MongoDB.Bson;

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

        //public TrainingGesture Get(string id)
        //{
        //    // Kiểm tra ObjectId hợp lệ nếu id dạng string (nâng cao)
        //    return _collection.Find(x => x.Id == id).FirstOrDefault();
        //}
        public TrainingGesture Get(string id)
        {
            // ✅ TC2: Validate id not null
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            // ✅ TC3: Validate id not empty
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            }

            // ✅ TC4: Validate id is valid ObjectId format
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _))
            {
                throw new ArgumentException($"Id '{id}' is not a valid ObjectId format.", nameof(id));
            }

            // ✅ TC7: Database operations with error handling
            try
            {
                var result = _collection.Find(x => x.Id == id).FirstOrDefault();

                // ✅ TC5, TC6: Check if result exists
                if (result == null)
                {
                    throw new KeyNotFoundException($"TrainingGesture with Id '{id}' not found.");
                }

                // ✅ TC1: Success
                return result;
            }
            catch (KeyNotFoundException)
            {
                // Re-throw KeyNotFoundException as is
                throw;
            }
            catch (MongoException ex)
            {
                // ✅ TC7: MongoDB error
                throw new InvalidOperationException($"Database error while retrieving TrainingGesture: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // ✅ TC7: Generic error
                throw new InvalidOperationException($"Failed to retrieve TrainingGesture: {ex.Message}", ex);
            }
        }
        public void Create(TrainingGesture trainingGesture)
        {
            // ✅ TC2: Validate object not null
            if (trainingGesture == null)
            {
                throw new ArgumentNullException(nameof(trainingGesture), "TrainingGesture object cannot be null.");
            }

            // ✅ TC8, TC9: Validate UserId not null/empty
            if (string.IsNullOrWhiteSpace(trainingGesture.UserId))
            {
                throw new ArgumentException("UserId is required.", nameof(trainingGesture.UserId));
            }

            // ✅ TC10: Validate UserId is valid ObjectId
            if (!MongoDB.Bson.ObjectId.TryParse(trainingGesture.UserId, out _))
            {
                throw new ArgumentException("UserId must be a valid ObjectId.", nameof(trainingGesture.UserId));
            }

            // ✅ TC11, TC12: Validate PoseLabel not null/empty
            if (string.IsNullOrWhiteSpace(trainingGesture.PoseLabel))
            {
                throw new ArgumentException("PoseLabel is required.", nameof(trainingGesture.PoseLabel));
            }

            // ✅ TC19: Validate TotalTrain >= 0
            if (trainingGesture.TotalTrain < 0)
            {
                throw new ArgumentException("TotalTrain cannot be negative.", nameof(trainingGesture.TotalTrain));
            }

            // ✅ TC21: Validate CorrectTrain >= 0
            if (trainingGesture.CorrectTrain < 0)
            {
                throw new ArgumentException("CorrectTrain cannot be negative.", nameof(trainingGesture.CorrectTrain));
            }

            // ✅ TC13: CorrectTrain không thể lớn hơn TotalTrain
            if (trainingGesture.CorrectTrain > trainingGesture.TotalTrain)
            {
                throw new ArgumentException("CorrectTrain cannot be greater than TotalTrain.");
            }

            // ✅ TC14, TC15: Validate Accuracy between 0 and 100
            if (trainingGesture.Accuracy < 0 || trainingGesture.Accuracy > 100)
            {
                throw new ArgumentException("Accuracy must be between 0 and 100.", nameof(trainingGesture.Accuracy));
            }

            // ✅ TC16: Accuracy đã là double nên không cần validate định dạng (type-safe)
            // Nếu pass sai type sẽ compile error

            // ✅ TC17, TC18: Validate VectorData not null/empty
            if (trainingGesture.VectorData == null)
            {
                throw new ArgumentException("VectorData is required.", nameof(trainingGesture.VectorData));
            }

            // TC20, TC22: TotalTrain và CorrectTrain đã là int nên type-safe

            // ✅ TC3, TC4: Check if all required fields are provided (already validated above)

            // ✅ TC5, TC6: Handle Id
            if (string.IsNullOrWhiteSpace(trainingGesture.Id) ||
                trainingGesture.Id == MongoDB.Bson.BsonString.Empty.ToString())
            {
                // MongoDB sẽ tự sinh Id nếu không có
                trainingGesture.Id = null;
            }

            // Set CreatedAt timestamp
            trainingGesture.CreateAt = DateTime.UtcNow;

            // ✅ TC23, TC7: Database operations with error handling
            try
            {
                _collection.InsertOne(trainingGesture);

                // ✅ TC1: Success
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // ✅ TC7: Duplicate Id error
                throw new InvalidOperationException($"A TrainingGesture with ID '{trainingGesture.Id}' already exists.", ex);
            }
            catch (MongoException ex)
            {
                // ✅ TC23: MongoDB error
                throw new InvalidOperationException($"Database error while creating TrainingGesture: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // ✅ TC23: Generic error
                throw new InvalidOperationException($"Failed to create TrainingGesture: {ex.Message}", ex);
            }
        }
        //public void Create(TrainingGesture trainingGesture)
        //{
        //    if (trainingGesture.Id == null || trainingGesture.Id == MongoDB.Bson.BsonString.Empty)
        //    {
        //        // Không gán gì để MongoDB tự sinh, hoặc gán GenerateNewId()
        //        // trainingGesture.Id = MongoDB.Bson.ObjectId.GenerateNewId();
        //    }
        //    trainingGesture.CreateAt = DateTime.Now;
        //    _collection.InsertOne(trainingGesture);
        //}
    }
}
