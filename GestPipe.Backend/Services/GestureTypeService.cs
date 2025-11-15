using GestPipe.Backend.Services.IServices;
using MongoDB.Driver;
using System.Collections.Generic;

public class GestureTypeService : IGestureTypeService
{
    private readonly IMongoCollection<GestureType> _gestureTypes;

    public GestureTypeService(IMongoDatabase database)
    {
        _gestureTypes = database.GetCollection<GestureType>("GestureType");
    }

    public List<GestureType> GetAll() => _gestureTypes.Find(_ => true).ToList();

    public GestureType GetById(string id) => _gestureTypes.Find(x => x.Id == id).FirstOrDefault();

    public void Create(GestureType gestureType) => _gestureTypes.InsertOne(gestureType);

    public void Update(string id, GestureType gestureType) =>
        _gestureTypes.ReplaceOne(x => x.Id == id, gestureType);

    public void Delete(string id) =>
        _gestureTypes.DeleteOne(x => x.Id == id);
}