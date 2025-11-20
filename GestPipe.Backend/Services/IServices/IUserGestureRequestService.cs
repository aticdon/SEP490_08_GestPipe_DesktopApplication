using GestPipe.Backend.Models;

namespace GestPipe.Backend.Services.IServices
{
    public interface IUserGestureRequestService
    {
        Task<UserGestureRequest> CreateRequestAsync(UserGestureRequest request);
        Task<List<UserGestureRequest>> GetPendingRequestsByUserAsync(string userId);
        Task<UserGestureRequest> GetByIdAsync(string id);
        Task<UserGestureRequest> GetLatestRequestByConfigIdAsync(string configId, string userId);
        Task<List<UserGestureRequest>> GetLatestRequestsByConfigIdsAsync(List<string> configIds, string userId);
        bool SetTrainingToSuccessful(string requestId);
        bool SetPendingToTraining(string requestId);
    }
}
