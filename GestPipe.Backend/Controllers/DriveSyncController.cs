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

        // POST /api/drivesync/sync-user/{userId}
        [HttpPost("sync-user/{userId}")]
        public async Task<IActionResult> SyncUser(string userId)
        {
            var count = await _driveService.SyncUserFolderAsync(userId);
            return Ok(new { downloaded = count });
        }
    }
}
