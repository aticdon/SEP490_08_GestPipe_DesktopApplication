using GestPipe.Backend.Models;
using GestPipe.Backend.Models.DTOs;
using GestPipe.Backend.Services;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserGestureRequestController : ControllerBase
    {
        private readonly IUserGestureRequestService _service;
        public UserGestureRequestController(IUserGestureRequestService service)
        {
            _service = service;
        }

        // POST: api/UserGestureRequest
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] UserGestureRequestDto dto)
        {
            var entity = new UserGestureRequest
            {
                UserId = dto.UserId,
                UserGestureConfigId = dto.UserGestureConfigId,
                GestureTypeId = dto.GestureTypeId,
                PoseLabel = dto.PoseLabel,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _service.CreateRequestAsync(entity);
            return Ok(result);
        }

        // GET: api/UserGestureRequest/list-pending/{userId}
        [HttpGet("list-pending/{userId}")]
        public async Task<ActionResult<List<UserGestureRequest>>> ViewListPendingRequestsByUser(string userId)
        {
            var requests = await _service.GetPendingRequestsByUserAsync(userId);
            return Ok(requests);
        }

        // GET: api/UserGestureRequest/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGestureRequest>> GetRequest(string id)
        {
            var req = await _service.GetByIdAsync(id);
            if (req == null) return NotFound();
            return Ok(req);
        }
        //[HttpGet("config/{configId}")]
        [HttpGet("user/{userId}/config/{configId}")]
        public async Task<ActionResult<UserGestureRequest>> GetLatestRequestByConfig(string configId, string userId)
        {
            var request = await _service.GetLatestRequestByConfigIdAsync(configId, userId);
            if (request == null) return NotFound();
            return Ok(request);
        }

        // ✅ THAY ĐỔI: Route mới với userId
        [HttpPost("user/{userId}/config/{configId}/start-training")]
        public IActionResult StartTraining(string userId, string configId)
        {
            Console.WriteLine($"[API] StartTraining - userId: {userId}, configId: {configId}");

            var ok = _service.SetPendingToTraining(configId, userId);

            if (!ok)
            {
                Console.WriteLine($"[API] StartTraining - Failed");
                return BadRequest("Update failed");
            }

            Console.WriteLine($"[API] StartTraining - Success");
            return NoContent();
        }

        [HttpPost("user/{userId}/config/{configId}/complete")]
        public IActionResult CompleteTraining(string userId, string configId)
        {
            Console.WriteLine($"[API] CompleteTraining - userId: {userId}, configId: {configId}");

            var ok = _service.SetTrainingToSuccessful(configId, userId);

            if (!ok)
            {
                Console.WriteLine($"[API] CompleteTraining - Failed");
                return BadRequest("Update failed");
            }

            Console.WriteLine($"[API] CompleteTraining - Success");
            return NoContent();
        }

        [HttpPost("user/{userId}/config/{configId}/cancel")]
        public IActionResult CancelCustom(string userId, string configId)
        {
            Console.WriteLine($"[API] CompleteTraining - userId: {userId}, configId: {configId}");

            var ok = _service.SetTrainingToCanceled(configId, userId);

            if (!ok)
            {
                Console.WriteLine($"[API] CompleteTraining - Failed");
                return BadRequest("Update failed");
            }

            Console.WriteLine($"[API] CompleteTraining - Success");
            return NoContent();
        }
        [HttpPost("batch/latest-requests")]
        public async Task<ActionResult<List<UserGestureRequest>>> GetLatestRequestsBatch([FromBody] LatestRequestBatchDto dto)
        {
            var latestRequests = await _service.GetLatestRequestsByConfigIdsAsync(dto.GestureConfigIds, dto.UserId);
            return Ok(latestRequests);
        }
    }
}