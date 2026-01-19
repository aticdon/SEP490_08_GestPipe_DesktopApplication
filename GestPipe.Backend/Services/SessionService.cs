using GestPipe.Backend.Models;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.IServices;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using System;

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
        //public void Create(Session session) => _sessions.InsertOne(session);
        public void Create(Session session)
        {
            // ✅ TC2: Validate session object not null
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session), "Session object cannot be null.");
            }

            // ✅ TC9, TC10: Validate UserId not null/empty
            if (string.IsNullOrWhiteSpace(session.UserId))
            {
                throw new ArgumentException("UserId is required.", nameof(session.UserId));
            }

            // ✅ TC11: Validate UserId is valid ObjectId
            if (!MongoDB.Bson.ObjectId.TryParse(session.UserId, out _))
            {
                throw new ArgumentException($"UserId '{session.UserId}' is not a valid ObjectId format.", nameof(session.UserId));
            }

            // ✅ TC12, TC13: Validate CategoryId not null/empty
            if (string.IsNullOrWhiteSpace(session.CategoryId))
            {
                throw new ArgumentException("CategoryId is required.", nameof(session.CategoryId));
            }

            // ✅ TC14: Validate CategoryId is valid ObjectId
            if (!MongoDB.Bson.ObjectId.TryParse(session.CategoryId, out _))
            {
                throw new ArgumentException($"CategoryId '{session.CategoryId}' is not a valid ObjectId format.", nameof(session.CategoryId));
            }

            // ✅ TC15, TC16: Validate TopicId not null/empty
            if (string.IsNullOrWhiteSpace(session.TopicId))
            {
                throw new ArgumentException("TopicId is required.", nameof(session.TopicId));
            }

            // ✅ TC17: Validate TopicId is valid ObjectId
            if (!MongoDB.Bson.ObjectId.TryParse(session.TopicId, out _))
            {
                throw new ArgumentException($"TopicId '{session.TopicId}' is not a valid ObjectId format.", nameof(session.TopicId));
            }

            // ✅ TC18: Validate Records not null
            if (session.Records == null)
            {
                throw new ArgumentException("Records cannot be null.", nameof(session.Records));
            }

            // ✅ TC19: Validate Records not empty
            if (session.Records.Count == 0)
            {
                throw new ArgumentException("Records must contain at least one entry.", nameof(session.Records));
            }

            // ✅ TC20: Validate Duration not negative
            if (session.Duration < 0)
            {
                throw new ArgumentException("Duration cannot be negative.", nameof(session.Duration));
            }

            // ✅ TC21: Validate Duration > 0 (nếu yêu cầu bắt buộc)
            if (session.Duration <= 0)
            {
                throw new ArgumentException("Duration must be greater than 0.", nameof(session.Duration));
            }

            // ✅ TC3, TC4: All fields validation (already covered above)

            // ✅ TC5, TC6, TC8: Handle Id
            if (string.IsNullOrWhiteSpace(session.Id))
            {
                // MongoDB will auto-generate if null
                session.Id = null;
            }
            else
            {
                // ✅ TC8: Validate Id format if provided
                if (!MongoDB.Bson.ObjectId.TryParse(session.Id, out _))
                {
                    throw new ArgumentException($"Id '{session.Id}' is not a valid ObjectId format.", nameof(session.Id));
                }
            }

            // Set CreatedAt if not provided
            if (session.CreatedAt == default)
            {
                session.CreatedAt = DateTime.UtcNow;
            }

            // ✅ TC22, TC7: Database operations with error handling
            try
            {
                _sessions.InsertOne(session);

                // ✅ TC1: Success
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // ✅ TC7: Duplicate Id error
                throw new InvalidOperationException($"A Session with Id '{session.Id}' already exists.", ex);
            }
            catch (MongoException ex)
            {
                // ✅ TC22: MongoDB error
                throw new InvalidOperationException($"Database error while creating Session: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // ✅ TC22: Generic error
                throw new InvalidOperationException($"Failed to create Session: {ex.Message}", ex);
            }
        }
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