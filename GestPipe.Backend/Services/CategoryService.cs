using GestPipe.Backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;

namespace GestPipe.Backend.Services
{
    public class CategoryService
    {
        private readonly IMongoCollection<Category> _categories;

        public CategoryService(IOptions<MongoDBSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.Database);
            _categories = database.GetCollection<Category>("Category");
        }

        public List<Category> GetAll() => _categories.Find(cat => true).ToList();
        public Category Get(string id) => _categories.Find(cat => cat.Id == id).FirstOrDefault();
        public void Create(Category cat) => _categories.InsertOne(cat);
    }
}