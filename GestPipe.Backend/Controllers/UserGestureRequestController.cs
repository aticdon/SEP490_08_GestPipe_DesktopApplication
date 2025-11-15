using GestPipe.Backend.Models;
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
            //// Nên set status là pending nếu client không truyền
            //if (dto.Status == null || !dto.Status.ContainsKey("main"))
            //    dto.Status = new Dictionary<string, string> { { "main", "pending" } };

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
        [HttpGet("config/{configId}")]
        public async Task<ActionResult<UserGestureRequest>> GetLatestRequestByConfig(string configId)
        {
            var request = await _service.GetLatestRequestByConfigIdAsync(configId);
            if (request == null) return NotFound();
            return Ok(request);
        }

        // POST /api/usergesturerequest/{id}/start-training
        [HttpPost("{id}/start-training")]
        public IActionResult StartTraining(string id)
        {
            var ok = _service.SetPendingToTraining(id);
            if (!ok) return BadRequest("Update failed");
            return NoContent();
        }

        // POST /api/usergesturerequest/{id}/complete
        [HttpPost("{id}/complete")]
        public IActionResult CompleteTraining(string id)
        {
            var ok = _service.SetTrainingToSuccessful(id);
            if (!ok) return BadRequest("Update failed");
            return NoContent();
        }
    }
}