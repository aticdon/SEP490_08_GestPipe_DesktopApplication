using GestPipe.Backend.Models;

namespace GestPipe.Backend.Services.IServices
{
    public interface ISessionService
    {
        void Create(Session session);
        List<Session> GetAll();
    }
}
