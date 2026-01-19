using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GestPipe.Backend.Tools
{
    public class MongoMigration
    {
        public static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: <connectionString> <databaseName> <defaultCulture>");
                Console.WriteLine("Example: mongodb://localhost:27017 GestPipe en-US");
                return;
            }

            var conn = args[0];
            var dbName = args[1];
            var defaultCulture = args[2];

            var client = new MongoClient(conn);
            var db = client.GetDatabase(dbName);
            var users = db.GetCollection<BsonDocument>("Users");

            var filter = Builders<BsonDocument>.Filter.Exists("language", false);
            var update = Builders<BsonDocument>.Update.Set("language", defaultCulture);
            var res = users.UpdateMany(filter, update);
            Console.WriteLine($"Matched: {res.MatchedCount}, Modified: {res.ModifiedCount}");
        }
    }
}