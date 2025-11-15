using GestPipe.Backend.Models;

namespace GestPipe.Backend.Services.IServices
{
    public interface IUserService
    {
        List<User> GetAll();
        User GetById(string id);
        void Create(User user);
        bool SetLanguage(string userId, string language);
        bool IncrementRequestCount(string userId);
        bool UpdateGestureRequestStatus(string userId, string status);
    }
}
