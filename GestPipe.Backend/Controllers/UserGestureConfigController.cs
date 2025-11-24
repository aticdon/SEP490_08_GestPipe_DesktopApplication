using GestPipe.Backend.Models;
using GestPipe.Backend.Models.DTOs;
using GestPipe.Backend.Services;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserGestureConfigController : ControllerBase
    {
        private readonly IUserGestureConfigService _service;
        private readonly IGestureTypeService _gestureTypeService;

        public UserGestureConfigController(IUserGestureConfigService service, IGestureTypeService gestureTypeService)
        {
            _service = service;
            _gestureTypeService = gestureTypeService;
        }

        // GET: api/UserGestureConfig/user/{userId}
        //[HttpGet("user/{userId}")]
        //public ActionResult<List<UserGestureConfig>> GetAllByUser(string userId)
        //{
        //    var configs = _service.GetAllByUserId(userId);
        //    if (configs == null || !configs.Any())
        //        return NotFound();
        //    return configs;
        //}
        //[HttpGet("user/{userId}")]
        //public ActionResult<List<UserGestureConfig>> GetAllByUser(string userId)
        //{
        //    var configs = _service.GetAllByUserId(userId);
        //    return Ok(configs ?? new List<UserGestureConfig>());
        //}
        [HttpGet("user/{userId}")]
        public ActionResult<List<UserGestureConfigDto>> GetAllByUser(string userId)
        {
            var configs = _service.GetAllByUserId(userId);
            var gestureTypes = _gestureTypeService.GetAll();
            var typeDict = gestureTypes.ToDictionary(gt => gt.Id, gt => gt.Code);
            var typeNameDict = gestureTypes.ToDictionary(gt => gt.Id, gt => gt.TypeName);

            var dtos = configs.Select(x => new UserGestureConfigDto
            {
                Id = x.Id,
                Name = typeDict.ContainsKey(x.GestureTypeId) ? typeDict[x.GestureTypeId] : new Dictionary<string, string>(),
                Type = typeNameDict.ContainsKey(x.GestureTypeId) ? typeNameDict[x.GestureTypeId] : new Dictionary<string, string>(),  // Action là nhãn động tác (tuỳ bạn muốn mapping gì)
                Accuracy = x.Accuracy,
                Status = x.Status,
                LastUpdate = x.UpdateAt
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/UserGestureConfig/{id}
        [HttpGet("{id}")]
        public ActionResult<UserGestureConfig> GetById(string id)
        {
            var config = _service.GetById(id);
            if (config == null)
                return NotFound();
            return config;
        }

        [HttpGet("{id:length(24)}/detail")]
        public ActionResult<GestureDetailsDto> GetDetail(string id)
        {
            var gesture = _service.GetById(id);
            if (gesture == null) return NotFound();

            var gestureType = _gestureTypeService.GetById(gesture.GestureTypeId);
            return new GestureDetailsDto
            {
                Id = gesture.Id,
                Name = gestureType?.Code ?? new Dictionary<string, string>(),
                PoseLabel = gesture.PoseLabel,
                Type = gestureType?.TypeName ?? new Dictionary<string, string>(),
                Accuracy = gesture.Accuracy,
                Status = gesture.Status,
                LastUpdate = gesture.UpdateAt,
                Description = "",
                VectorData = gesture.VectorData
            };
        }

        [HttpPost("import-from-csv")]
        public async Task<IActionResult> ImportFromCsv([FromBody] ImportGestureFromCsvRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.UserId) ||
                string.IsNullOrWhiteSpace(request.CsvContent))
            {
                return BadRequest("UserId và CsvContent không được rỗng.");
            }

            var count = await _service.ImportFromCsvAsync(request.UserId, request.CsvContent);

            return Ok(new
            {
                inserted = count
            });
        }
    }
}