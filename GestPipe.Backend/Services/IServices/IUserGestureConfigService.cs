using GestPipe.Backend.Models;

namespace GestPipe.Backend.Services.IServices
{
    public interface IUserGestureConfigService
    {
        List<UserGestureConfig> GetAllByUserId(string userId);
        UserGestureConfig GetById(string id);

        Task<int> ImportFromCsvAsync(string userId, string csvContent);
    }
}
