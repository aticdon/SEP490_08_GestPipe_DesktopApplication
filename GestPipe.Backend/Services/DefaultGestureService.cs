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

}