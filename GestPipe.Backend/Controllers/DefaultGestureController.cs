using GestPipe.Backend.Models;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DefaultGestureController : ControllerBase
{
    private readonly IDefaultGestureService _defaultGestureService;
    private readonly IGestureTypeService _gestureTypeService;

    public DefaultGestureController(IDefaultGestureService defaultGestureService, IGestureTypeService gestureTypeService)
    {
        _defaultGestureService = defaultGestureService;
        _gestureTypeService = gestureTypeService;
    }

    [HttpGet]
    public ActionResult<List<DefaultGesture>> Get() => _defaultGestureService.GetAll();

    [HttpGet("{id:length(24)}")]
    public ActionResult<DefaultGesture> Get(string id)
    {
        var gesture = _defaultGestureService.GetById(id);
        if (gesture == null) return NotFound();
        return gesture;
    }

    [HttpPost]
    public ActionResult<DefaultGesture> Create(DefaultGesture gesture)
    {
        _defaultGestureService.Create(gesture);
        return CreatedAtAction(nameof(Get), new { id = gesture.Id }, gesture);
    }

    [HttpPut("{id:length(24)}")]
    public IActionResult Update(string id, DefaultGesture gestureIn)
    {
        var gesture = _defaultGestureService.GetById(id);
        if (gesture == null) return NotFound();
        _defaultGestureService.Update(id, gestureIn);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public IActionResult Delete(string id)
    {
        var gesture = _defaultGestureService.GetById(id);
        if (gesture == null) return NotFound();
        _defaultGestureService.Delete(id);
        return NoContent();
    }

    [HttpGet("display")]
    public async Task<ActionResult<List<DefaultGestureDto>>> GetDisplay()
    {
        // Lấy tất cả DefaultGestures
        var gestures = _defaultGestureService.GetAll();

        // Lấy tất cả GestureTypes
        var gestureTypes = _gestureTypeService.GetAll();
        var typeDict = gestureTypes.ToDictionary(gt => gt.Id, gt => gt.Code);
        var typeNameDict = gestureTypes.ToDictionary(gt => gt.Id, gt => gt.TypeName);

        // Ghép dữ liệu
        var result = gestures.Select(g => new DefaultGestureDto
        {
            Id = g.Id,
            Name = typeDict.ContainsKey(g.GestureTypeId) ? typeDict[g.GestureTypeId] : "",
            Type = typeNameDict.ContainsKey(g.GestureTypeId) ? typeNameDict[g.GestureTypeId] : "", 
            Accuracy = g.Accuracy,
            Status = g.Status,
            LastUpdate = g.CreatedAt
        }).ToList();

        return Ok(result);
    }
    [HttpGet("{id:length(24)}/detail")]
    public ActionResult<GestureDetailsDto> GetDetail(string id)
    {
        var gesture = _defaultGestureService.GetById(id);
        if (gesture == null) return NotFound();

        var gestureType = _gestureTypeService.GetById(gesture.GestureTypeId);
        return new GestureDetailsDto
        {
            Id = gesture.Id,
            Name = gestureType?.Code ?? "",
            PoseLabel = gesture.PoseLabel,
            Type = gestureType?.TypeName ?? "",
            Accuracy = gesture.Accuracy,
            Status = gesture.Status,
            LastUpdate = gesture.CreatedAt,
            Description = "",
            VectorData = gesture.VectorData
        };
    }
}