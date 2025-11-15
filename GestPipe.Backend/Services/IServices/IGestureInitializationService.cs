using GestPipe.Backend.Models;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services.IServices
{
    public interface IGestureInitializationService
    {
        /// <summary>
        /// Initialize user gestures from default gestures
        /// </summary>
        Task<bool> InitializeUserGesturesAsync(string userId);

        /// <summary>
        /// Get gesture statistics for user
        /// </summary>
        Task<GestureStats> GetUserGestureStatsAsync(string userId);

        /// <summary>
        /// Check if user already has gestures initialized
        /// </summary>
        Task<bool> HasUserGesturesAsync(string userId);
    }
}