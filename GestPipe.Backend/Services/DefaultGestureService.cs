using GestPipe.Backend.Services.IServices;
using MongoDB.Driver;

public class DefaultGestureService : IDefaultGestureService
{
    private readonly IMongoCollection<DefaultGesture> _defaultGestures;

    public DefaultGestureService(IMongoDatabase database)
    {
        _defaultGestures = database.GetCollection<DefaultGesture>("DefaultGestures");
    }

    public List<DefaultGesture> GetAll() => _defaultGestures.Find(_ => true).ToList();

    public DefaultGesture GetById(string id) => _defaultGestures.Find(x => x.Id == id).FirstOrDefault();

    public void Create(DefaultGesture gesture) => _defaultGestures.InsertOne(gesture);

    public void Update(string id, DefaultGesture gesture) =>
        _defaultGestures.ReplaceOne(x => x.Id == id, gesture);

    public void Delete(string id) =>
        _defaultGestures.DeleteOne(x => x.Id == id);
}