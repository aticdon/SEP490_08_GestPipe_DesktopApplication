using GestPipe.Backend.Models;

namespace GestPipe.Backend.Services.IServices
{
    public interface IUserGestureConfigService
    {
        List<UserGestureConfig> GetAllByUserId(string userId);
        UserGestureConfig GetById(string id);
        //void Create(UserGestureConfigDto config);
        //void Update(string id, UserGestureConfigDto config);
        //void Delete(string id);
    }
}
