using GestPipe.Backend.Models;

namespace GestPipe.Backend.Services.IServices
{
    public interface IDefaultGestureService
    {
        List<DefaultGesture> GetAll();
        DefaultGesture GetById(string id);
        //void Create(DefaultGesture gesture);
        //void Update(string id, DefaultGesture gesture);
        //void Delete(string id);

    }
}
