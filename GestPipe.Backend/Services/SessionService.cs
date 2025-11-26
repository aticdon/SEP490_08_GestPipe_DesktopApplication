using GestPipe.Backend.Models;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.IServices;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;

namespace GestPipe.Backend.Services
{
    public class SessionService : ISessionService
    {
        private readonly IMongoCollection<Session> _sessions;

        //public SessionService(IOptions<MongoDbSettings> mongoSettings)
        public SessionService(IMongoDatabase database)
        {
            //var client = new MongoClient(mongoSettings.Value.ConnectionString);
            //var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _sessions = database.GetCollection<Session>("Session");
        }
        public void Create(Session session) => _sessions.InsertOne(session);
        public List<Session> GetAll()
        {
            try
            {
                return _sessions.Find(session => true).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("MongoDB error: " + ex.Message);
                throw;
            }
        }

    }
}