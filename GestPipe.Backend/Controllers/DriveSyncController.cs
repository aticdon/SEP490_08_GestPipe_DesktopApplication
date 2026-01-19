using GestPipe.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriveSyncController : ControllerBase
    {
        private readonly GoogleDriveService _driveService;

        public DriveSyncController(GoogleDriveService driveService)
        {
            _driveService = driveService;
        }

        [HttpPost("sync-user/{userId}")]
        public IActionResult SyncUser(string userId)
        {
            _ = Task.Run(() => _driveService.SyncUserFolderAsync(userId));
            return Accepted(new { message = "Sync started" });
        }

        [HttpGet("sync-user/progress/{userId}")]
        public IActionResult GetSyncProgress(string userId)
        {
            var progress = _driveService.GetProgress(userId);
            return Ok(progress);
        }
        [HttpPost("upload-user/{userId}")]
        public IActionResult UploadUser(string userId)
        {
            _ = Task.Run(() => _driveService.UploadUserFolderAsync(userId));
            return Accepted(new { message = "Upload started" });
        }
        [HttpGet("upload-user/progress/{userId}")]
        public IActionResult GetUploadProgress(string userId)
        {
            var progress = _driveService.GetUploadProgress(userId);
            return Ok(progress);
        }

    }
}
